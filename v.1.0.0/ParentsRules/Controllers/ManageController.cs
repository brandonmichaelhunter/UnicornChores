using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ParentsRules.Models;
using ParentsRules.Models.ManageViewModels;
using ParentsRules.Services;
using ParentsRules.Data;

namespace ParentsRules.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly UrlEncoder _urlEncoder;
        private readonly ApplicationDbContext _context;

        private const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public ManageController(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          IEmailSender emailSender,
          ILogger<ManageController> logger,
          UrlEncoder urlEncoder, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _urlEncoder = urlEncoder;
            _context = context;
            
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var model = new IndexViewModel()
                {
                    Username = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    IsEmailConfirmed = user.EmailConfirmed,
                    StatusMessage = StatusMessage
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }
        #region Friends Page
        [HttpGet]
        public async Task<IActionResult> Friends()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                /* Query the user's friends. */
                ViewData["Friends"] = GetUserFriends(user.Id).AsEnumerable();

                /* Query the user's active friend requests. */
                ViewData["FriendRequests"] = GetUserFriendRequests(user.Id).AsEnumerable();

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        private List<FriendViewModel> GetUserFriends(string id)
        {
            try
            {
                /* Query the AccountAssociations to find the out the friends of the current user.  */
                List<AccountAssociations> usersFriendsMap = _context.AccountAssociations.Where(a => a.PrimaryUserID == id && a.IsChild == false).ToList<AccountAssociations>();
                if(usersFriendsMap.Count > 0)
                {
                    List<FriendViewModel> userFriends = new List<FriendViewModel>();
                    
                    // Process through the userFriendsMap list
                    foreach(AccountAssociations acct in usersFriendsMap)
                    {
                        var AccountUser = _context.AccountUsers.Where(a => a.Id == acct.AssociatedUserID).FirstOrDefault();
                        if(AccountUser != null)
                        {
                            var account = new FriendViewModel()
                            {
                                FirstName = AccountUser.FirstName,
                                MiddleName = AccountUser.MiddleName,
                                LastName = AccountUser.LastName,
                                Email = AccountUser.Email,
                                Username = AccountUser.UserName,
                                FriendSince = (_context.UserConformationRequests.Where(b => b.Email == AccountUser.Email && b.IsConfirmed == 1).First()).DateConfirmed
                            };
                            userFriends.Add(account);
                        }
                    }
                    return userFriends;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private List<FriendsRequestViewModel> GetUserFriendRequests(string id)
        {
            try
            {
                List<FriendsRequestViewModel> friendRequests = new List<FriendsRequestViewModel>();
                List<UserConformationRequests> requests = _context.UserConformationRequests.Where(a => a.RequestedUserID == id && a.IsConfirmed == 0).ToList<UserConformationRequests>();
                if (requests.Count > 0)
                {
                    foreach (UserConformationRequests request in requests)
                    {
                        var friendRequest = new FriendsRequestViewModel()
                        {
                            DateRequested = request.DateSent,
                            FirstName = request.FirstName,
                            LastName = request.LastName,
                            Email = request.Email,
                            StatusMessage = StatusMessage
                        };
                        friendRequests.Add(friendRequest);
                    }
                    return friendRequests;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return null;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendFriendRequest(IndexViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }
                /* Send the friend request via email to your friend.*/
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.AssociatedUserLink(user.Id, code, Request.Scheme);
                var associatedUserEmail = model.Email;
                var associatedUserFirstName = model.AssociatedUserFirstName;
                var associatedUserLastName = model.AssociatedUserLastName;
                string websiteurl = Url.Action("Index", "Home");
                await _emailSender.SendParentRequestEmailConfirmationAsync(associatedUserEmail, callbackUrl, user.FirstName, associatedUserFirstName, websiteurl);
                /* Save the request to database */

                var request = new UserConformationRequests
                {
                    Email = associatedUserEmail,
                    DateSent = DateTime.Now,
                    IsConfirmed = 0,
                    ExpiredDate = DateTime.Now.AddDays(7),
                    RequestedUserID = user.Id,
                    FirstName = associatedUserFirstName,
                    LastName = associatedUserLastName,
                    RegistrationCode = code
                };
                _context.UserConformationRequests.Add(request);
                _context.SaveChanges();
                StatusMessage = "Parent link request sent. Please have then check their email your email.";
                return RedirectToAction(nameof(Friends));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }
        #endregion
        #region Friend Requests Page - Old Code Need to Remove
        [HttpGet]
        public async Task<IActionResult> FriendRequests()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }



                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        

       
        #endregion

        #region Children Page
        /* Display children list */
        private List<ChildrenViewModel> GetChildren(string ParentID)
        {
            try
            {
                List<ChildrenViewModel> retList = new List<ChildrenViewModel>();
                
                List<AccountAssociations> accounts = _context.AccountAssociations.Where(a => a.PrimaryUserID == ParentID && a.IsChild == true).ToList<AccountAssociations>();
                if (accounts.Count > 0)
                {
                    /* Evaluate the parents kids list.*/
                    foreach (AccountAssociations account in accounts){
                        /* Get the child's profile. */
                        var child = _context.AccountUsers.Where(b => b.Id == account.AssociatedUserID).FirstOrDefault();
                        if(child != null){
                            var childProfile = new ChildrenViewModel(){ FirstName = child.FirstName, MiddleName = child.MiddleName, LastName = child.LastName, Username = child.UserName };
                            retList.Add(childProfile);
                        }
                    }
                    return retList;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return null;
            }

        }
        [HttpGet]
        public async Task<IActionResult> Children()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            
            /* Query for the parent kids.  */
            List<ChildrenViewModel> childrenList = GetChildren(user.Id);
            ViewData["ChildrenList"] = childrenList.AsEnumerable();
            return View();
            
        }
        // Child Regiration 
        public async Task<IActionResult> ChildRegistration(ChildrenViewModel model)
        {
            try
            {
                /* Get the parent profile. */
                var parent = await _userManager.GetUserAsync(User);
                if (parent == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                /* Create user account. */
                var child = new ApplicationUser { UserName = model.Username, Email = parent.Email, FirstName = model.FirstName, MiddleName = model.MiddleName, LastName = model.LastName, IsChild = "1" };
                var result = await _userManager.CreateAsync(child, model.Password);
                if (result.Succeeded)
                {
                    /* Automatically confirm the childs email.*/
                    child.EmailConfirmed = true;
                    _context.AccountUsers.Update(child);
                    _context.SaveChanges();

                    /* Associate the child with their parent. */
                    var PrimaryUserAssociation = new AccountAssociations() { PrimaryUserID = parent.Id, AssociatedUserID = child.Id, IsChild = true };
                    _context.AccountAssociations.Add(PrimaryUserAssociation);
                    _context.SaveChanges();
                }
                //}
                return RedirectToAction(nameof(Children));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }

        }

        #endregion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var email = user.Email;
                if (model.Email != email)
                {
                    var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                    if (!setEmailResult.Succeeded)
                    {
                        throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                    }
                }

                var phoneNumber = user.PhoneNumber;
                if (model.PhoneNumber != phoneNumber)
                {
                    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                    if (!setPhoneResult.Succeeded)
                    {
                        throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                    }
                }

                StatusMessage = "Your profile has been updated";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationEmail(IndexViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                var email = user.Email;
                string websiteurl = Url.Action("Index", "Home");
                string userDisplayName = string.Format("{0}", User.Identity.Name);
                await _emailSender.SendEmailConfirmationAsync(userDisplayName, model.Email, callbackUrl, websiteurl);

                StatusMessage = "Verification email sent. Please check your email.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var hasPassword = await _userManager.HasPasswordAsync(user);
                if (!hasPassword)
                {
                    return RedirectToAction(nameof(SetPassword));
                }

                var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    AddErrors(changePasswordResult);
                    return View(model);
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation("User changed their password successfully.");
                StatusMessage = "Your password has been changed.";

                return RedirectToAction(nameof(ChangePassword));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
           
        }

        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var hasPassword = await _userManager.HasPasswordAsync(user);

                if (hasPassword)
                {
                    return RedirectToAction(nameof(ChangePassword));
                }

                var model = new SetPasswordViewModel { StatusMessage = StatusMessage };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (!addPasswordResult.Succeeded)
                {
                    AddErrors(addPasswordResult);
                    return View(model);
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                StatusMessage = "Your password has been set.";

                return RedirectToAction(nameof(SetPassword));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLogins()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var model = new ExternalLoginsViewModel { CurrentLogins = await _userManager.GetLoginsAsync(user) };
                model.OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                    .Where(auth => model.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                    .ToList();
                model.ShowRemoveButton = await _userManager.HasPasswordAsync(user) || model.CurrentLogins.Count > 1;
                model.StatusMessage = StatusMessage;

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkLogin(string provider)
        {
            try
            {
                // Clear the existing external cookie to ensure a clean login process
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

                // Request a redirect to the external login provider to link a login for the current user
                var redirectUrl = Url.Action(nameof(LinkLoginCallback));
                var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
                return new ChallengeResult(provider, properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
           
        }

        [HttpGet]
        public async Task<IActionResult> LinkLoginCallback()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var info = await _signInManager.GetExternalLoginInfoAsync(user.Id);
                if (info == null)
                {
                    throw new ApplicationException($"Unexpected error occurred loading external login info for user with ID '{user.Id}'.");
                }

                var result = await _userManager.AddLoginAsync(user, info);
                if (!result.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred adding external login for user with ID '{user.Id}'.");
                }

                // Clear the existing external cookie to ensure a clean login process
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

                StatusMessage = "The external login was added.";
                return RedirectToAction(nameof(ExternalLogins));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var result = await _userManager.RemoveLoginAsync(user, model.LoginProvider, model.ProviderKey);
                if (!result.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred removing external login for user with ID '{user.Id}'.");
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                StatusMessage = "The external login was removed.";
                return RedirectToAction(nameof(ExternalLogins));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> TwoFactorAuthentication()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var model = new TwoFactorAuthenticationViewModel
                {
                    HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                    Is2faEnabled = user.TwoFactorEnabled,
                    RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user),
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> Disable2faWarning()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                if (!user.TwoFactorEnabled)
                {
                    throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");
                }

                return View(nameof(Disable2fa));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable2fa()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
                if (!disable2faResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");
                }

                _logger.LogInformation("User with ID {UserId} has disabled 2fa.", user.Id);
                return RedirectToAction(nameof(TwoFactorAuthentication));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
                if (string.IsNullOrEmpty(unformattedKey))
                {
                    await _userManager.ResetAuthenticatorKeyAsync(user);
                    unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
                }

                var model = new EnableAuthenticatorViewModel
                {
                    SharedKey = FormatKey(unformattedKey),
                    AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey)
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                // Strip spaces and hypens
                var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

                var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                    user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

                if (!is2faTokenValid)
                {
                    ModelState.AddModelError("model.Code", "Verification code is invalid.");
                    return View(model);
                }

                await _userManager.SetTwoFactorEnabledAsync(user, true);
                _logger.LogInformation("User with ID {UserId} has enabled 2FA with an authenticator app.", user.Id);
                return RedirectToAction(nameof(GenerateRecoveryCodes));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        [HttpGet]
        public IActionResult ResetAuthenticatorWarning()
        {
            return View(nameof(ResetAuthenticator));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetAuthenticator()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                await _userManager.SetTwoFactorEnabledAsync(user, false);
                await _userManager.ResetAuthenticatorKeyAsync(user);
                _logger.LogInformation("User with id '{UserId}' has reset their authentication app key.", user.Id);

                return RedirectToAction(nameof(EnableAuthenticator));
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                if (!user.TwoFactorEnabled)
                {
                    throw new ApplicationException($"Cannot generate recovery codes for user with ID '{user.Id}' as they do not have 2FA enabled.");
                }

                var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
                var model = new GenerateRecoveryCodesViewModel { RecoveryCodes = recoveryCodes.ToArray() };

                _logger.LogInformation("User with ID {UserId} has generated new 2FA recovery codes.", user.Id);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("{0} - Error Message: {1}", System.Reflection.MethodBase.GetCurrentMethod(), ex.Message));
                return RedirectToAction("Index", "StatusCode", 500);
            }
            
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenicatorUriFormat,
                _urlEncoder.Encode("ParentsRules"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        #endregion
    }
}
