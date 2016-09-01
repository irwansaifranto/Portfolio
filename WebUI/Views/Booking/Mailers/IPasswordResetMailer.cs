using Mvc.Mailer;
using WebUI.Mailers.Models;

namespace WebUI.Mailers
{ 
    public interface IPasswordResetMailer
    {
			MvcMailMessage PasswordReset(MailerModel model);
	}
}