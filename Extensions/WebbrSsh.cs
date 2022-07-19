using System;
using Chilkat;
using Task = System.Threading.Tasks.Task;

namespace Webbr.Extensions
{
    public interface IWebbrSsh
    {
        Task ExecOperatorCommand(string sshHost, int sshPort, string sshUser, string sshPassword, string sshCommand);
        string SshCommandExecute(string sshHost, int sshPort, string sshUser, string sshCommand);
    }

    public class WebbrSsh : IWebbrSsh
    {
        #region SshCommandExecute
        public string SshCommandExecute(string sshHost, int sshPort, string sshUser, string sshCommand)
        {
            #region technicalKey
            var technicalKeyPassphrase = "A&#OH6AG53";
            var technicalKey = @"-----BEGIN RSA PRIVATE KEY-----
Proc-Type: 4,ENCRYPTED
DEK-Info: AES-128-CBC,13B424706C18DEC521CCBC2AE4CE98DF

YM4pfULoCXigYDwnIL7ZaG2D1Sjr7kewCR6hG6vKV5GpolctpjGYZEKBevqiQ2ml
AzE/wapm1K5XTF/k81rEnbB9ZbddPidIKxSFqjsJnhEgId0Kobw6AhzefuTD2V65
4dJ85mbZ1CSwMOg8GUFPB65NRPOb3XzHYfVXVKw+RFL5/qea+XDFPOj7i7P50dmD
J7C7DWngXX6iiDwiAbIui1lOIxtFqSNBlAheyCgOVq1MH17FZFB98+Cv9JCIwqg8
qrkFOKcblFyoCX4Xw9mkWa83tai94CVZWozUCVs4OHskE2g1WZrOf1VGj/IAdvpM
HeQErGsZq6AB1LzMaZ+AErt7rlhSfztBmofepvFmCicBcy7oYcZilyoUrkaYF9tg
J+VcPly/kucWCViGLL1wXcDcgjQJi4vv/W9LbtToeLasqCAPALbS5GlHbwYbUgEP
YLEWZ/MTJGi3JOvHyvkZrT2jAAIUt4jlFf7h35aTdmoKlHtvsZxgiLR2YYWbPbUJ
kOnSfX14KugiemiVahipqXNneSH4o0GADrhUhhg4LNwHJVY7n6N5WwhuHP/Ty85F
4h813DHwgZ+8aIxLHJO8m4T8ZBq/W6usaTrOoA3sQO1qrJtpKVwPy1YedvnR1ArF
Tlx2qDJMJluU/aL+w7guZAJqgqtOTibbI9fHAnOvp04/TlmjburSORI+tLm6p7OJ
UMW8cjbuH8PqQZYdal+hwqAsM34dbgdcU4JnNO1fkzPVAYSNPBWpYDoNgArBSgOj
AfEtO8e9hxaxdDQV46nJBPhJrvY7AJcCV5ocb7hge8pFQKV6LZ7nBYY4c0+sj7In
HbYT2g4RzdL95N05yRPNJGOWKqA2YutSOQbUZ/Z6AqIIXOVTOIzQDp1DEbhgwbuN
2QIZgEuWuo4BTewNXZNcPjTI3F4ewiUGRynrAzOqmIsqhk0FmLLGDBHQPU/G862w
aHtrjyLlGRVVK0qcT/nl5gmUeJewFxu6Otr2oYyXIm0w5k06JYLwHqesIs/9CNLt
8iWd0Dr7XhMaRzFJH3O9O5ItoSpoIHWRCxYah3+lABBDVdb3WJRTtaID0bDs4wnx
BjFz8u9jlhkB56YHvl8/XZgjNIJT78dHw5Ci28mHSEN9M4GwE0Sh3jcFnCkJbnL0
CjLeBASlr3J1jSFRSc4YszHePOeAJ6IkYUUpSlAdT/rmwlFJqNYkfQ3iKK+KihpO
n4/54xiIGZU2uvHVGdcAsq3WoV3PLwGzWuTzDCZB6d67j/NN+9UpWs43gOHL6deU
47pHj3KKx+7A+o36SkLLO+GzZgemmmbwMNX5KLbZcMPAC5TdceOmTXGAlIxEmEjn
jpQRrjo+NtlR+HhAP88d/RPQppB+Mpw5TAC/FwWYB4Wn2IFv1Ri3P+jkagD2wCWj
Rv8oc5MZ4P3/hFNx6G38kt1VsnVtu+yD/QtpVLMc76ljHx70R39rK6hf1MWKy7lZ
9RSJQdNffcrZkeIGjezSQWjdCpVhG5xnQeumLIvcxkZkyFDK9ZYi9g+HKTgrzAuj
gLzp+YhQuEM1xH1N9zfXc3Kbf/MC858zc0e0KswwCi3rf98luglMshZ+WyeR1e5t
8xKUUdqusoiyNaOUo1sSdMNeQ2G35n6pTnczjUlpMLc6JihMjSh89hthqm59Hndd
wSLTixgUonBdpI4P6b/+kuQl5icZxbtgtKOw0NBxdVVztOzjsuGytT2KUZ5V5+oo
cLg3byRDBTggHtaF63Gs3vV4rvjoGIht56yYw0TOBlNpEMsP3rCTE0H5EoaBylcH
eKpuSvyIDseZrsxCCb54OknVzD8OxvMOG4JpXnZm8173dQxMKvIr4s4CaIl4Lj/Y
TGMYGIzXAAmwqLB7/ekH3NlsE29RVIJHnk3TyxjTbuHV4/ENA/NVD1Lb/K4y1Hm9
ttwen7XS0BokKgh5eofJeXoBkWbfXx6icq+xH7xrLOzXJCUtVfbhT9cjKGq7i/Q5
YqCSJH2MrAwVSIc0YBFXkvGdoG4mY7WicX+O7R3GI5qJGqeHY2zDzsNTDkWYkN9y
Px2PICtw+61Xm06/Kq/qJY2Gcl4gvdj5HeHMNSqhzSCXHbNnnLA1i5uyo6f9k4yx
zSqaSrmNDZwEoeG5jB1hn5S/RiLGMe3Yjq/8xdrfYFfGKwITcRyvJJKHHBq5k/1+
xC4wOlmoj2VrrUZOE3EIgEHeHYY+o+iP+yqvWq4snHqNw73AlCUErePrUcd+pHMO
6vi3KYV+4blemFjxWnMMtlY8H+O9GvngKVRUThFB2Uy9chhLoifiXJTpgB8DJ7Gp
NWr6oxxRrggBRt4MXF61YHOcJH0rlrbmTQbMChqUqspn0vQwQl/QICOupWV81P8h
p76Mb5lWsoyzXzCp+WsPK+S6gHpGDL8VYU4rYnPBbNxsHnKeZWHzMp6R+rcX8QcR
ZYpcDerekMgwh/qzN5ohbR6LJghls8k/IIT2ogIM0+ZAOE+qX0pfG3OTqIgL+czf
0H2SlOuWoFmKqJktv6jhAm6Hpk45MpTra5KF6QzROV+ebCfZpC6gHtZUnwQqIyv3
D0MsGLZEYi3kuStjPJ7SO3A1mc4iWiOtVnWqpqYQ5qfGhhxMoLM9DxfpNpIgGyXr
OcgvuLa6bKSxvEuILpyfd75XvsXa2BTTTOCTNf0PtJznorfWFBoUjuZkQXZURbtM
t/m+W5NuLn09bcrUqtYfdjj7eioK+Ppkgf8gbzr4qci0/L95ex+1O9bFw+8zZx0d
Kb4wwdgrqtuDS52Bx6ReI4Qu6MYL0Lkcjfz8hOi3e9P9F1+0fLQCbj7axyhgTY4s
fTwMm2ZWckWT3Z00V0BORq4xrUp7zjTItJyfNxpDvT7D4xW+W+zT2RcNd4KR2u+Q
jPVrDbCgbK0f6N7TO3r8UgeIS+DVpyBQlqVFZqgYBNAdoOVw5VkATqyMXYi/F0gd
n/kpf8k8SU8gKp0oONr1BwzwKg+7rOQODFUX2se/+MeB2jlNaKrwoqaEwZplGy3U
EM7DchK+MXPTX1QdEG+f4AVisuhbYtanI0wAFchn9kenD9HN42OhaeGKqB7dTuGr
b1iFd7uBCak+N8+8pE1LKfwVlsPKtAEj/DZJW35uT46DCYnHIjYtYS9F2jngRbr8
-----END RSA PRIVATE KEY-----";
            #endregion

            var key = technicalKey;
            var keyPassphrase = technicalKeyPassphrase;
            
            var chilkatGlob = new Global();
            chilkatGlob.UnlockBundle("4QLgRU.CBX0424_VD96FZ2P3f7n");
            
            var ssh = new Ssh();
            var conn = ssh.Connect(sshHost, sshPort);
            if (!conn) throw new Exception($"Не удалось подключиться к серверу {sshHost}");
            
            var sshKey = new SshKey();
            sshKey.Password = keyPassphrase;
            sshKey.FromOpenSshPrivateKey(key);
            
            var auth = ssh.AuthenticatePk(sshUser, sshKey);
            if (!auth) throw new Exception($"Не удалось авторизоваться на сервере {sshHost}");
            
            var commandOutput = ssh.QuickCommand(sshCommand,"ansi");
            
            ssh.Disconnect();
            
            return commandOutput;
        }

        #endregion

        #region ExecOperatorCommand
        public async Task ExecOperatorCommand(string sshHost, int sshPort, string sshUser, string sshPassword, string sshCommand)
        {
            var chilkatGlob = new Global();
            chilkatGlob.UnlockBundle("4QLgRU.CBX0424_VD96FZ2P3f7n");
            
            var ssh = new Ssh();
            var conn = ssh.Connect(sshHost, sshPort);
            if (!conn) throw new Exception($"Не удалось подключиться к серверу {sshHost}");
            
            var auth = ssh.AuthenticatePw(sshUser, sshPassword);
            if (!auth) throw new Exception($"Не удалось авторизоваться на сервере {sshHost}");
            
            ssh.QuickCommand(sshCommand,"ansi");
            
            ssh.Disconnect();
        }
        #endregion
    }
}