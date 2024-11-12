using System.Collections.Generic;
using System;

public class LoginManager
{
    private const int MaxAttempts = 3;
    private const int LockoutDuration = 60; // Låsningsperiod 60 seknunder

    private Dictionary<string, (int attempts, DateTime? lockoutEndTime)> loginAttempts =
        new Dictionary<string, (int attempts, DateTime? lockoutEndTime)>();

    public void RecordFailedAttempt(string username)
    {
        if (!loginAttempts.ContainsKey(username))
        {
            loginAttempts[username] = (1, null);
        }
        else
        {
            var (attempts, lockoutEndTime) = loginAttempts[username];
            attempts++;

            if (attempts >= MaxAttempts)
            {
                Console.WriteLine("För många misslyckade försök. Konto har låst sig i en minut.");
                loginAttempts[username] = (attempts, DateTime.Now.AddSeconds(LockoutDuration));
            }
            else
            {
                loginAttempts[username] = (attempts, lockoutEndTime);
            }
        }
    }

    public bool IsLockedOut(string username)
    {
        if (loginAttempts.ContainsKey(username))
        {
            var (attempts, lockoutEndTime) = loginAttempts[username];
            if (lockoutEndTime.HasValue && lockoutEndTime > DateTime.Now)
            {
                TimeSpan remaining = lockoutEndTime.Value - DateTime.Now;
                Console.WriteLine($"Konto låst. försök igen om {remaining.Seconds} sekunder.");
                return true;
            }

            // Låsningsperioden har avslutats återställ försök
            if (lockoutEndTime.HasValue && lockoutEndTime > DateTime.Now)
            {
                ResetAttempts(username);

            }
        }
        return false;
    }

    public void ResetAttempts(string username)
    {
        loginAttempts[username] = (0, null);
    }
}
        
