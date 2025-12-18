@page "/register"
@using CollegeEventsBlazor.Models
@using CollegeEventsBlazor.Services
@inject AuthService Auth
@inject NavigationManager Nav

<div class="ce-page">
    <section class="ce-auth">
        <div class="ce-auth__card ce-card">
            <div class="ce-auth__top">
                <div class="ce-hero__badge">
                    <span class="ce-dot"></span>
                    <span>Create your account</span>
                </div>

                <h1 class="ce-auth__title">Register</h1>
                <p class="ce-muted">Fill in your details to create a new student account.</p>
            </div>

            @if (!string.IsNullOrWhiteSpace(_error))
            {
                <div class="ce-alert ce-alert--error" style="margin-top:1rem;">
                    <div class="ce-alert__title">Registration failed</div>
                    <div class="ce-muted">@_error</div>
                </div>
            }

            @if (!string.IsNullOrWhiteSpace(_ok))
            {
                <div class="ce-alert ce-alert--ok" style="margin-top:1rem;">
                    <div class="ce-alert__title">Success</div>
                    <div class="ce-muted">@_ok</div>
                </div>
            }

            <div class="ce-auth__form">
                <div class="ce-field">
                    <label class="ce-label">Student Number</label>
                    <input class="ce-input"
                           placeholder="Student Number"
                           @bind="_studentNumber"
                           @bind:event="oninput" />
                </div>

                <div class="ce-field">
                    <label class="ce-label">Full Name</label>
                    <input class="ce-input"
                           placeholder="Full Name"
                           @bind="_fullName"
                           @bind:event="oninput" />
                </div>

                <div class="ce-field">
                    <label class="ce-label">Email</label>
                    <input class="ce-input"
                           type="email"
                           placeholder="Email"
                           @bind="_email"
                           @bind:event="oninput" />
                </div>

                <div class="ce-field">
                    <label class="ce-label">Password</label>
                    <input class="ce-input"
                           type="password"
                           placeholder="••••••••"
                           @bind="_password"
                           @bind:event="oninput" />
                    <div class="ce-muted ce-help">Use at least 6 characters (recommended: 8+).</div>
                </div>

                <div class="ce-auth__actions">
                    <button class="ce-btn ce-btn--primary"
                            @onclick="DoRegister"
                            disabled="@_busy">
                        @(_busy ? "Creating..." : "Create Account")
                    </button>

                    <button class="ce-btn ce-btn--outline"
                            @onclick="@(() => Nav.NavigateTo("/login"))"
                            disabled="@_busy">
                        I already have an account
                    </button>
                </div>

                <div class="ce-auth__footer">
                    <button class="ce-btn ce-btn--ghost"
                            @onclick="@(() => Nav.NavigateTo("/"))"
                            disabled="@_busy">
                        ← Back to Home
                    </button>
                </div>
            </div>
        </div>

        <aside class="ce-auth__side ce-card">
            <h3 class="ce-h2" style="margin:0;">What happens next?</h3>
            <hr class="ce-sep" />
            <ul class="ce-muted" style="margin:0; padding-left:1.1rem; line-height:1.7;">
                <li>After registering, you’ll go to the <b>Login</b> page.</li>
                <li>Then you can register for events in the <b>Student Portal</b>.</li>
                <li>If you see an error, check your API is running and CORS is correct.</li>
            </ul>
        </aside>
    </section>
</div>

@code {
    private string _studentNumber = "";
    private string _fullName = "";
    private string _email = "";
    private string _password = "";

    private string _error = "";
    private string _ok = "";
    private bool _busy = false;

    private async Task DoRegister()
    {
        _error = "";
        _ok = "";
        _busy = true;

        try
        {
            await Auth.RegisterAsync(new RegisterRequest
            {
                StudentNumber = _studentNumber.Trim(),
                FullName = _fullName.Trim(),
                Email = _email.Trim(),
                Password = _password
            });

            _ok = "Registered successfully. Redirecting to login...";
           
            await Task.Delay(500);
            Nav.NavigateTo("/login");
        }
        catch (Exception ex)
        {
            _error = ex.Message;
        }
        finally
        {
            _busy = false;
        }
    }
}
