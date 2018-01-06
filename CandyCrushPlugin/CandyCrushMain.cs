using System;
using System.Collections.Generic;
using System.Text;
using Fiddler;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.IO;
using System.Collections;
using Newtonsoft.Json;
using System.Xml;


// Extension requires Fiddler 2.2.8.6+ because it uses types introduced in v2.2.8...
[assembly: Fiddler.RequiredVersion("2.2.8.6")]

namespace CandyCrushPlugin
{
    public class CandyCrushMain : IFiddlerExtension, IAutoTamper
    {
        public FormMain frmMain;
        public CandyCrushMain()
        {
            frmMain = new FormMain();
        }

        public void OnBeforeUnload()
        {
            //throw new NotImplementedException();

        }

        public void OnLoad()
        {
            //throw new NotImplementedException();
            frmMain.Show();
        }

        public void AutoTamperRequestAfter(Session oSession)
        {
            //throw new NotImplementedException();
        }

        public void AutoTamperRequestBefore(Session oSession)
        {
            //throw new NotImplementedException();
        }

        public void AutoTamperResponseAfter(Session oSession)
        {
/*            string hostname = oSession.hostname;
            //frmMain.dbgLine(hostname);
            if (hostname.IndexOf("game.shadowera.com") >= 0)
            {
                string url = oSession.url;
                //frmMain.dbgLine(url);
                int index;
                if ((index = url.IndexOf(".php?")) >= 0)
                {
                    String queryStr = url.Substring(index + 5);
                    String link = url.Substring(0, index);
                    //this.frmMain.dbgLine(queryStr);
                    NameValueCollection nv = Utilities.ParseQueryString(queryStr);
                    String edata = nv["edata"];
                    String token = nv["token"];

                    String response = oSession.GetResponseBodyAsString();
                    edata = Uri.UnescapeDataString(edata);
                    if (token.Length == 0)
                    {
                        //frmMain.dbgLine("edata=" + edata);
                        //frmMain.dbgLine("response=" + response);
                        edata = aes_decrypt(edata, default_loginKey);
                        response = aes_decrypt(response, default_loginKey);
                        String[] lines = edata.Split('\n');
                        String name = lines[0];

                        lines = response.Split('\n');
                        if (lines[0].Equals("OK"))
                        {
                            String sessionToken = lines[13];
                            String loginKey = lines[14];
                            TokenSession tok = new TokenSession();
                            tok.login = name;
                            tok.key = loginKey;
                            nvTokenKey[sessionToken] = tok;
                        }
                        frmMain.dbgLine("\n------------Begin " + link + "-----------");
                        frmMain.dbgLine(edata);
                        frmMain.dbgLine("\n------------Reply-----------");
                        frmMain.dbgLine(response);
                        frmMain.dbgLine("\n------------End-----------");

                    }
                    else
                    {
                        // check if we have correct login key
                        if (nvTokenKey[token] != null)
                        {
                            edata = aes_decrypt(edata, nvTokenKey[token].key);
                            response = aes_decrypt(response, nvTokenKey[token].key);
                            frmMain.dbgLine("\n------------Begin " + link + "-----------");
                            frmMain.dbgLine(edata);
                            frmMain.dbgLine("\n------------Reply-----------");
                            frmMain.dbgLine(response);
                            frmMain.dbgLine("\n------------End-----------");
                            processResponse(response, link);
                        }
                        else
                        {
                            edata = "";
                            frmMain.dbgLine("Skip decode don't have key");
                        }
                    }
                }
            }*/
        }

        public void processResponse(String reponse, String link)
        {

        }

        public void AutoTamperResponseBefore(Session oSession)
        {
            //http://candycrush.king.com/candycrushapi/getGameConfigurations
            String hostname = oSession.hostname;
            if (hostname.IndexOf("candycrush.king.com") >= 0)
            {
                string url = oSession.url;
                if (url.IndexOf("getGameConfigurations") > 0)
                {
                    frmMain.dbgLine("Config");
                    String response = oSession.GetResponseBodyAsString();
                    String newResponse = processGameConfig(response);
                    oSession.utilSetResponseBody(newResponse);
                }
                if (url.IndexOf("gameInit") > 0)
                {
                    frmMain.dbgLine("Init");
                    String response = oSession.GetResponseBodyAsString();
                    String newResponse = processGameInit(response);
                    //oSession.utilSetResponseBody(newResponse);
                }
            }

        }

        public void OnBeforeReturningError(Session oSession)
        {
            //throw new NotImplementedException();
        }

        public String processGameConfig(String body)
        {
            body = body.Replace(@"numberOfColours\"":5", @"numberOfColours\"":4");
            body = body.Replace(@"numberOfColours\"":6", @"numberOfColours\"":4");
            frmMain.dbgLine("numberOfColours = 4");
            return (body);
        }

        public String processGameInit(String body){
            String jsonstr = @"{""root"":" + body + "}";
            XmlDocument d = JsonConvert.DeserializeXmlNode(jsonstr);

            /* remove condition */
            XmlNodeList lst = d.SelectNodes(@"//unlockConditions[type='FriendInviteUnlockCondition']");
            List<XmlNode> l = new List<XmlNode>();
            foreach (XmlNode n in lst)
            {
                //txtDebug.AppendText(n.OuterXml + Environment.NewLine);
                l.Add(n);
            }
            foreach (XmlNode n in l)
            {
                n.ParentNode.RemoveChild(n);
            }
            frmMain.dbgLine("Remove FriendInviteUnlockCondition " + l.Count + " places");

            // set level up

            lst = d.SelectNodes(@"//unlocked");
            foreach (XmlNode n in lst)
            {
                //txtDebug.AppendText(n.OuterXml + Environment.NewLine);
                n.InnerText = "true";
            }

            //----------------------------------------
            lst = d.SelectNodes(@"//episodes[id='3']");
            foreach (XmlNode n in lst)
            {
                //txtDebug.AppendText(n.OuterXml + Environment.NewLine);
                n.ParentNode.RemoveChild(n);
                //n.InnerText = "true";                    
            }
            lst = d.SelectNodes(@"//episodes[id='2']");

            XmlNode newNode = lst[0].CloneNode(true);
            newNode["id"].InnerText = "3";
            lst[0].ParentNode.AppendChild(newNode);
            //----------------------------------------

            //lst = d.SelectNodes(@"//currentUser");
            //lst[0]["immortal"].InnerText = "true";
            //frmMain.dbgLine("immortal");


            String newResponse = JsonConvert.SerializeXmlNode(d);
            if (newResponse.StartsWith(@"{""root"":"))
            {
                newResponse = newResponse.Substring(8);
                newResponse = newResponse.Substring(0, newResponse.Length - 1);
            }
            return (newResponse);
            //txtDebug.AppendText(newResponse);
        }
    }
}
