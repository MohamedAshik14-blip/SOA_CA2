let jwtToken = null;
let currentStudentNumber = null;
let toastTimeout = null;

// --------- UI Helpers ---------

function setStatus(message, isError = false) {
    const toast = document.getElementById("status");
    toast.textContent = message;
    toast.classList.toggle("error", isError);
    toast.classList.add("show");

    if (toastTimeout) clearTimeout(toastTimeout);
    toastTimeout = setTimeout(() => {
        toast.classList.remove("show");
    }, 3500);
}

// View navigation
function switchView(viewId) {
    // hide all views
    document.querySelectorAll(".view").forEach(v => v.classList.remove("view-active"));
    // show selected
    const view = document.getElementById(viewId);
    if (view) view.classList.add("view-active");

    // update nav button active state
    document.querySelectorAll(".nav-link").forEach(btn => {
        btn.classList.toggle("active", btn.dataset.view === viewId);
    });
}

// Update nav when logged in/out
function updateNavAuth() {
    const navUser = document.getElementById("navUser");
    const navAuthBtn = document.getElementById("navAuthBtn");

    if (jwtToken) {
        navUser.textContent = currentStudentNumber || "Student";
        navAuthBtn.textContent = "Logout";
        navAuthBtn.onclick = logout;
    } else {
        navUser.textContent = "Guest";
        navAuthBtn.textContent = "Login / Register";
        navAuthBtn.onclick = openAuthModal;
    }
}

function logout() {
    jwtToken = null;
    currentStudentNumber = null;
    updateNavAuth();
    setStatus("You have been logged out.");
}

// Short token helper (if you want to show it somewhere)
function shortToken(token) {
    if (!token) return "Not logged in";
    if (token.length <= 20) return token;
    return token.substring(0, 20) + "...";
}

// --------- Auth modal controls ---------

function openAuthModal() {
    const backdrop = document.getElementById("authModal");
    backdrop.classList.remove("hidden");
    backdrop.classList.add("visible");
    showLogin();
}

function closeAuthModal() {
    const backdrop = document.getElementById("authModal");
    backdrop.classList.remove("visible");
    setTimeout(() => backdrop.classList.add("hidden"), 200);
}

function backdropClick(event) {
    if (event.target.id === "authModal") {
        closeAuthModal();
    }
}

function showLogin() {
    document.getElementById("loginForm").classList.remove("hidden");
    document.getElementById("registerForm").classList.add("hidden");
    document.getElementById("tabLogin").classList.add("active");
    document.getElementById("tabRegister").classList.remove("active");
    document.getElementById("authModalTitle").textContent = "Login to your account";
}

function showRegister() {
    document.getElementById("loginForm").classList.add("hidden");
    document.getElementById("registerForm").classList.remove("hidden");
    document.getElementById("tabLogin").classList.remove("active");
    document.getElementById("tabRegister").classList.add("active");
    document.getElementById("authModalTitle").textContent = "Create a student account";
}

// --------- API Helper ---------

async function apiFetch(url, options = {}, requireAuth = false) {
    const opts = { ...options };
    opts.headers = {
        "Content-Type": "application/json",
        ...(options.headers || {})
    };

    if (requireAuth) {
        if (!jwtToken) {
            setStatus("You must log in first.", true);
            throw new Error("Not logged in");
        }
        opts.headers["Authorization"] = "Bearer " + jwtToken;
    }

    const response = await fetch(url, opts);
    if (!response.ok) {
        const text = await response.text();
        let msg = text || response.statusText;
        // remove quotes if ASP.NET returns plain string
        if (msg.startsWith("\"") && msg.endsWith("\"")) {
            msg = msg.substring(1, msg.length - 1);
        }
        throw new Error(msg);
    }

    const contentType = response.headers.get("Content-Type") || "";
    if (contentType.includes("application/json")) {
        return await response.json();
    }

    return await response.text();
}

// --------- AUTH ---------

async function register() {
    try {
        const body = {
            studentNumber: document.getElementById("regStudentNumber").value.trim(),
            fullName: document.getElementById("regFullName").value.trim(),
            email: document.getElementById("regEmail").value.trim(),
            password: document.getElementById("regPassword").value
        };

        if (!body.studentNumber || !body.fullName || !body.email || !body.password) {
            setStatus("Please fill in all registration fields.", true);
            return;
        }

        await apiFetch("/api/Auth/register", {
            method: "POST",
            body: JSON.stringify(body)
        });

        setStatus("Registration successful. You can now log in.");
        showLogin();
    } catch (e) {
        setStatus("Registration failed: " + e.message, true);
    }
}

async function login() {
    try {
        const body = {
            studentNumber: document.getElementById("loginStudentNumber").value.trim(),
            password: document.getElementById("loginPassword").value
        };

        if (!body.studentNumber || !body.password) {
            setStatus("Please enter student number and password.", true);
            return;
        }

        const token = await apiFetch("/api/Auth/login", {
            method: "POST",
            body: JSON.stringify(body)
        });

        jwtToken = token;
        currentStudentNumber = body.studentNumber;
        updateNavAuth();
        closeAuthModal();
        setStatus("Login successful. Welcome, " + body.studentNumber + "!");
        loadMyRegistrations().catch(() => {});
    } catch (e) {
        setStatus("Login failed: " + e.message, true);
    }
}

// --------- EVENTS ---------

async function loadEvents() {
    try {
        const events = await apiFetch("/api/Events", { method: "GET" });

        const list = document.getElementById("eventsList");
        const homePreview = document.getElementById("homeEventsPreview");

        list.innerHTML = "";
        homePreview.innerHTML = "";

        if (!events.length) {
            list.innerHTML = "<p class='muted small'>No events yet. Create one on the left.</p>";
            homePreview.innerHTML = "<p class='muted small'>No upcoming events.</p>";
            return;
        }

        // Full events list
        events.forEach(ev => {
            const li = document.createElement("li");
            li.className = "list-item";

            const header = document.createElement("div");
            header.className = "list-item-header";

            const title = document.createElement("span");
            title.textContent = `#${ev.id} • ${ev.title}`;

            const capacity = document.createElement("span");
            capacity.className = "badge";
            capacity.textContent = `${ev.capacity} seats`;

            header.appendChild(title);
            header.appendChild(capacity);

            const desc = document.createElement("div");
            desc.className = "small muted";
            desc.textContent = ev.description || "No description";

            const actions = document.createElement("div");
            actions.style.marginTop = "0.35rem";

            const btn = document.createElement("button");
            btn.className = "btn-outline";
            btn.textContent = "Register for this event";
            btn.onclick = () => {
                if (!jwtToken) {
                    openAuthModal();
                } else {
                    document.getElementById("regEventId").value = ev.id;
                    switchView("accountView");
                }
            };

            actions.appendChild(btn);

            li.appendChild(header);
            li.appendChild(desc);
            li.appendChild(actions);

            list.appendChild(li);
        });

        // Home preview (first 2 events)
        const previewItems = events.slice(0, 2);
        previewItems.forEach(ev => {
            const p = document.createElement("p");
            p.innerHTML = `<strong>${ev.title}</strong> — capacity ${ev.capacity}`;
            homePreview.appendChild(p);
        });

        if (events.length > 2) {
            const extra = document.createElement("p");
            extra.className = "muted small";
            extra.textContent = `+ ${events.length - 2} more event(s) available.`;
            homePreview.appendChild(extra);
        }

        setStatus("Events loaded.");
    } catch (e) {
        setStatus("Failed to load events: " + e.message, true);
    }
}

async function createEvent() {
    try {
        const body = {
            title: document.getElementById("eventTitle").value.trim(),
            description: document.getElementById("eventDescription").value.trim(),
            startTime: document.getElementById("eventStart").value,
            endTime: document.getElementById("eventEnd").value,
            capacity: parseInt(document.getElementById("eventCapacity").value, 10)
        };

        if (!body.title || !body.startTime || !body.endTime || !body.capacity) {
            setStatus("Please fill in title, times and capacity.", true);
            return;
        }

        await apiFetch("/api/Events", {
            method: "POST",
            body: JSON.stringify(body)
        }, true);

        setStatus("Event created successfully.");
        document.getElementById("eventTitle").value = "";
        document.getElementById("eventDescription").value = "";
        document.getElementById("eventStart").value = "";
        document.getElementById("eventEnd").value = "";
        document.getElementById("eventCapacity").value = 50;

        await loadEvents();
    } catch (e) {
        setStatus("Failed to create event: " + e.message, true);
    }
}

// --------- REGISTRATIONS ---------

async function registerForEvent() {
    try {
        const eventId = parseInt(document.getElementById("regEventId").value, 10);
        if (!eventId || eventId <= 0) {
            setStatus("Please enter a valid event ID.", true);
            return;
        }

        const body = { eventId };

        await apiFetch("/api/Registrations", {
            method: "POST",
            body: JSON.stringify(body)
        }, true);

        setStatus(`Registered for event #${eventId}.`);
        document.getElementById("regEventId").value = "";
        await loadMyRegistrations();
    } catch (e) {
        setStatus("Failed to register: " + e.message, true);
    }
}

async function loadMyRegistrations() {
    try {
        const regs = await apiFetch("/api/Registrations/my", { method: "GET" }, true);
        const list = document.getElementById("myRegistrationsList");
        list.innerHTML = "";

        if (!regs.length) {
            list.innerHTML = "<p class='muted small'>You have no registrations yet.</p>";
            return;
        }

        regs.forEach(r => {
            const li = document.createElement("li");
            li.className = "list-item";

            const header = document.createElement("div");
            header.className = "list-item-header";

            const title = document.createElement("span");
            title.textContent = `Event #${r.eventId} • ${r.eventTitle}`;

            const status = document.createElement("span");
            status.className = "badge";
            if (r.checkOutTime) {
                status.textContent = "Completed";
            } else if (r.checkInTime) {
                status.textContent = "Checked in";
            } else {
                status.textContent = "Registered";
            }

            header.appendChild(title);
            header.appendChild(status);

            const times = document.createElement("div");
            times.className = "small muted";
            times.textContent = `Registered: ${r.registeredAt}`;

            const more = document.createElement("div");
            more.className = "small muted";
            more.textContent = `Check-in: ${r.checkInTime || "-"} | Check-out: ${r.checkOutTime || "-"}`;

            const actions = document.createElement("div");
            actions.style.marginTop = "0.35rem";
            actions.style.display = "flex";
            actions.style.gap = "0.5rem";

            const btnCheckIn = document.createElement("button");
            btnCheckIn.className = "btn-outline";
            btnCheckIn.textContent = "Check in";
            btnCheckIn.onclick = () => checkIn(r.eventId);

            const btnCheckOut = document.createElement("button");
            btnCheckOut.className = "btn-outline";
            btnCheckOut.textContent = "Check out";
            btnCheckOut.onclick = () => checkOut(r.eventId);

            actions.appendChild(btnCheckIn);
            actions.appendChild(btnCheckOut);

            li.appendChild(header);
            li.appendChild(times);
            li.appendChild(more);
            li.appendChild(actions);

            list.appendChild(li);
        });

        setStatus("Registrations loaded.");
    } catch (e) {
        setStatus("Failed to load registrations: " + e.message, true);
    }
}

async function checkIn(eventId) {
    try {
        await apiFetch(`/api/Registrations/${eventId}/checkin`, {
            method: "POST"
        }, true);
        setStatus(`Checked in to event #${eventId}.`);
        await loadMyRegistrations();
    } catch (e) {
        setStatus("Check-in failed: " + e.message, true);
    }
}

async function checkOut(eventId) {
    try {
        await apiFetch(`/api/Registrations/${eventId}/checkout`, {
            method: "POST"
        }, true);
        setStatus(`Checked out of event #${eventId}.`);
        await loadMyRegistrations();
    } catch (e) {
        setStatus("Check-out failed: " + e.message, true);
    }
}

// --------- Initialisation ---------

window.addEventListener("DOMContentLoaded", () => {
    // Wire up nav buttons
    document.querySelectorAll(".nav-link").forEach(btn => {
        btn.addEventListener("click", () => {
            const viewId = btn.dataset.view;
            if (viewId) {
                switchView(viewId);
            }
        });
    });

    // Hero "Browse Events" button uses data-view-link, already handled below
    document.addEventListener("click", (e) => {
        const link = e.target.closest("[data-view-link]");
        if (link) {
            const viewId = link.getAttribute("data-view-link");
            if (viewId) switchView(viewId);
        }
    });

    updateNavAuth();
    switchView("homeView");
    loadEvents().catch(() => {});
});
