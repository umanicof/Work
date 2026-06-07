using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace NkfLib
{
    public static partial class FileIOUtil
    {
        /// <summary>
        /// 指定ディレクトリに全ユーザのフルコントロール権限を付与する
        /// </summary>
        public static void SetPermissionAllUsersFullControl(string path)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) return;

#if false
            // This gets the "Authenticated Users" group, no matter what it's called
            SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
#else
            // バッチファイルでのアクセスに失敗する場合があるので、Everyoneを使用
            var sid = new NTAccount("everyone");
#endif

            // Create the rules
            FileSystemAccessRule writerule = new FileSystemAccessRule(
                sid, 
                FileSystemRights.FullControl,
#if true // [このフォルダ、サブフォルダおよびファイル]
                InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                PropagationFlags.None,
#endif
                AccessControlType.Allow);

            // Get your file's ACL
            var info = new DirectoryInfo(path);
            DirectorySecurity fsecurity = info.GetAccessControl();

            // Add the new rule to the ACL
            fsecurity.AddAccessRule(writerule);

            // Set the ACL back to the file
            info.SetAccessControl(fsecurity);
        }
    }
}
