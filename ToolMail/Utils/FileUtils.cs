using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolMail.Models;

namespace ToolMail.Utils
{
    public class FileUtils
    {
        public static List<InputMail> TEXT_getAccount(string path)
        {
            string rawAccount = TEXT_readTextFile(path);
            List<string> listRawAccount = TEXT_splitEnter(rawAccount);

            List<InputMail> listAccount = new List<InputMail>();
            listRawAccount.ForEach((account) =>
            {
                listAccount.Add(TEXT_removeSlashed(account));
            });

            return listAccount;
        }
        public static string TEXT_readTextFile(string path)
        {
            return System.IO.File.ReadAllText(path).Trim();
        }

        public static InputMail TEXT_removeSlashed(string str)
        {
            string[] arr = str.Split(new string[] { "|" }, StringSplitOptions.None);
            return new InputMail(arr[0], arr[1]);
        }

        public static List<string> TEXT_splitEnter(string str)
        {
            str = str.Replace("\r", "");
            string[] arr = str.Split(new string[] { "\n" }, StringSplitOptions.None);
            return new List<string>(arr).Where(item => item.Trim() != "").ToList();
        }

        public static void TEXT_writeTextFile(string path, InputMail account)
        {
            string content = account.Email + "|" + account.Password + "\n";
            System.IO.File.WriteAllText(path, content);
        }
    }
}
