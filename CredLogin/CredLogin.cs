using ActiveDirectoryAccess;
using CredentialsLibrary;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace CredLogin
{
    public class CredLogin
    {


        List<GroupPrincipal> _berechtigteGruppen;
        string _user;
        string _pw;

        public CredLogin(List<GroupPrincipal> gp)
        {
            _berechtigteGruppen = gp;
        }


        public CredLogin(GroupPrincipal gp)
        {
            _berechtigteGruppen.Add(gp);
        }

        public CredLogin(string grpsamname,string domain)
        {
            AdAccess ad = new AdAccess(domain);
            _berechtigteGruppen = ad.GetGroups(new object[] { grpsamname }, new int[] { (int)AdAccess.UserFilter.SamAccountName });

        }
        public List<GroupPrincipal> BerechtigteGruppen { get => _berechtigteGruppen; set => _berechtigteGruppen = value; }

        public static bool LoginCheck(string user, string pw, string domain)
        {
            try
            {
                PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain);

                // validate the credentials
                
                return pc.ValidateCredentials(user, pw);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Fehler",MessageBoxButtons.OK,MessageBoxIcon.Error);
                throw;
            }
        
        }

        public bool Login(string name, string headername)
        {
            bool value = false;
            try
            {
                CredentialsDialog dialog = new CredentialsDialog(headername);

                if (name != null) dialog.AlwaysDisplay = true; // prevent an infinite loop
                if (dialog.Show() == DialogResult.OK)
                {
                    this._user = dialog.Name;
                    this._pw = dialog.Password;
                    if (Authenticate())
                    {
                        value = true;
                        if (dialog.SaveChecked) dialog.Confirm(true);
                    }
                    else
                    {
                        try
                        {
                            dialog.Confirm(false);
                        }
                        catch (ApplicationException applicationException)
                        {
                     
                        }

                        if (MessageBox.Show("Login nicht erfolgreich", "Login fehlgeschlagen", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                            value = Login(dialog.Name, headername); // need to find a way to display 'Logon unsuccessful'
                    }
                }
            }
            catch (ApplicationException applicationException)
            {
                MessageBox.Show(applicationException.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return value;
        }

        private bool Authenticate()
        {
            AdAccess ad;
            bool vaild = false;

            string[] splitted_user = _user.Split(Path.DirectorySeparatorChar);

            ad = splitted_user.Length > 1 ? new AdAccess(splitted_user[0]) : new AdAccess(System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName);

            if (LoginCheck(_user, _pw, ad.Domain))
            {
                foreach (GroupPrincipal g in _berechtigteGruppen)
                {
                    vaild = ad.IsMemberof(ad.GetUser("conr"), g, true);
                    if(vaild)
                    {
                        break;
                    }
                }
            }

            return vaild;
        }
    }
}

