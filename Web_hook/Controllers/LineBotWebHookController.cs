using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using isRock.LineBot.Conversation;
using isRock.LineBot;
using System.IO;
using System.Reflection;
namespace Web_hook.Controllers
{
    public class LineBotWebHookController : isRock.LineBot.LineWebHookControllerBase
    {
        [Route("api/LineBotWebHook")]
        [HttpPost]
        public IHttpActionResult POST()
        {
            string ChannelAccessToken = "gUSAa5blXyICLt1BrPhuyDIFsIbyeon02SqIttttXgHh2guG42y/k82r4uWByV1Sz8uG5xokgCw9GBPFH3Oci9x/Rs18oRrWQYmCrUO5YQ+7JzBsabL9rz4n8AoV75soyaURDZjwVjoklqAIXhmDjFGUYhWQfeY8sLGRXgo3xvw=";
            string Message = "";
            string postData = Request.Content.ReadAsStringAsync().Result;
            //剖析JSON
            var ReceivedMessage = isRock.LineBot.Utility.Parsing(postData);

            Bot bot = new Bot(ChannelAccessToken);
            try
            {
                var item = this.ReceivedMessage.events.FirstOrDefault();
             
                switch (item.type)
                {
                    case "join":
           
                        Message = $"有人把我加入{item.source.type}中了，大家好啊~";
                       

                        //回覆用戶
                        isRock.LineBot.Utility.ReplyMessage(ReceivedMessage.events[0].replyToken, Message, ChannelAccessToken);
                        break;
                    case "message":


                          if (item.message.type == "text")
                          {
                              string userid = "";
                              LineUserInfo UserInfo = null;
                              if (item.source.type.ToLower() == "room")
                              {
                                  UserInfo = isRock.LineBot.Utility.GetRoomMemberProfile(
                                       item.source.roomId, item.source.userId, ChannelAccessToken);
                                  userid = item.source.roomId;
                              }
                              if (item.source.type.ToLower() == "group")
                              {
                                  UserInfo = isRock.LineBot.Utility.GetGroupMemberProfile(
                                        item.source.groupId, item.source.userId, ChannelAccessToken);
                                  userid = item.source.groupId;
                              }
                              if (item.source.type.ToLower() == "user")
                              {
                                  var UserID = ReceivedMessage.events.FirstOrDefault().source.userId;
                                  UserInfo = isRock.LineBot.Utility.GetUserInfo(UserID, ChannelAccessToken);
                                  userid = UserInfo.userId;
                              }



                              if (item.message.text == "bye")
                              {
                                  //回覆用戶
                                  bot.ReplyMessage(ReceivedMessage.events[0].replyToken, "掰噗");
                                  //離開
                                  if (item.source.type.ToLower() == "room")
                                      isRock.LineBot.Utility.LeaveRoom(item.source.roomId, ChannelAccessToken);
                                  if (item.source.type.ToLower() == "group")
                                      isRock.LineBot.Utility.LeaveGroup(item.source.groupId, ChannelAccessToken);

                                  break;
                              }
                              if (item.message.text == "!情感分析")
                              {
                               
                                bot.ReplyMessage(ReceivedMessage.events[0].replyToken, "https://sentimentclassifier--j11pl4.repl.co/");

                                
                            }
                           
                            if (item.message.text == "!Game")
                              {
                                  TextMessage msg = new TextMessage("請選一個遊戲");
                                  msg.quickReply.items.Add(new QuickReplyMessageAction(
                                      "猜拳", "猜拳", new Uri("https://hahow-production.imgix.net/58f03ffc4909c907004ab7c8?w=1000&auto=format&s=92d94131a1f984e6120868b3b077a310.png")));
                                  msg.quickReply.items.Add(new QuickReplyMessageAction(
                                      "終極密碼", "終極密碼", new Uri("https://is2-ssl.mzstatic.com/image/thumb/Purple114/v4/66/98/fe/6698fe77-75ad-6dbe-9e00-d4915888278f/AppIcon-0-1x_U007emarketing-0-0-85-220-4.png/246x0w.png")));



                                  bot.ReplyMessage(ReceivedMessage.events[0].replyToken, msg);
                              }
                              if (item.message.text == "猜拳")
                              {
                                  TextMessage msg = new TextMessage("請出一個拳");
                                  msg.quickReply.items.Add(new QuickReplyMessageAction(
                                       "剪刀", "剪刀", new Uri("https://img1.momoshop.com.tw/goodsimg/0004/765/799/4765799_B.jpg?t=1513059445.png")));
                                  msg.quickReply.items.Add(new QuickReplyMessageAction(
                                      "石頭", "石頭", new Uri("https://cdn.shopify.com/s/files/1/0744/0467/products/magis_piedras_08S_360x358.jpeg?v=1422958763.png")));
                                  msg.quickReply.items.Add(new QuickReplyMessageAction(
                                      "布", "布", new Uri("https://i1.wp.com/navi.love/wp-content/uploads/2017/06/05%E9%AD%94%E5%B8%83%E5%AD%B8%E7%94%9F%E6%8A%B9%E5%B8%83%E9%99%84%E6%8E%9B%E8%80%B3.jpg?fit=290%2C270&ssl=1.png")));

                                  bot.ReplyMessage(ReceivedMessage.events[0].replyToken, msg);
                              }
                              if (item.message.text == "剪刀" || item.message.text == "石頭" || item.message.text == "布")
                              {
                                  string msg = "";
                                  int gamer = 0;
                                  switch (item.message.text)
                                  {
                                      case "剪刀":

                                          gamer = 2;
                                          break;
                                      case "石頭":
                                          gamer = 0;
                                          break;
                                      case "布":
                                          gamer = 1;
                                          break;
                                      default:
                                          break;
                                  }

                                  Mora mora = new Mora(gamer);
                                  if (UserInfo != null)
                                  {
                                      mora.who.userid = UserInfo.userId;
                                      msg = UserInfo.displayName + mora.search_member(mora.who);
                                  }
                                  bot.ReplyMessage(ReceivedMessage.events[0].replyToken, msg);
                              }
                              if (item.message.text == "終極密碼")
                              {

                                  bot.ReplyMessage(ReceivedMessage.events[0].replyToken, "請猜0-1000的數字,並加一個!例如:!666");

                              }




                              if (item.message.text.Substring(0, 1) == "!")
                              {
                                  string i = item.message.text.Remove(0, 1);
                                  int n;
                                  if (int.TryParse(i, out n))
                                  {
                                      if (Int32.Parse(i) >= 0 && Int32.Parse(i) <= 1000)
                                      {
                                          Ultimate ultimate = new Ultimate(userid, Int32.Parse(i));

                                          ultimate.who.max = 1000;
                                          ultimate.who.min = 0;
                                          bot.ReplyMessage(ReceivedMessage.events[0].replyToken, UserInfo.displayName + ":" + ultimate.get_result(ultimate.who));
                                      }
                                  }
                              }
                          }
                        

                        break;
                    default:
                        break;
                }

                //回覆API OK
                return Ok();
            }
            catch (Exception ex)
            {
                //請自行處理Exception
                isRock.LineBot.Utility.ReplyMessage(
                ReceivedMessage.events[0].replyToken, "ERROR:" + ex.Message, ChannelAccessToken);

                return Ok();
            }
        }
    }


    public class Mora
    {
        public int Gamer;
        public int AI;
        public  int win;
        public  int lose;
        public  int draw;
        public string msg;
        public struct Gamer_data
        {
            public string userid;
            public int win;
            public int lose;
            public int draw;
          
        }
        public static Gamer_data[] member = new Gamer_data[50];

        public Gamer_data who = new Gamer_data();
        


    public Mora(int gamer)
        {
             
            Gamer = gamer;
            Random random = new Random();
            AI=random.Next(0, 3);
        }
        //0=石頭  1=布  2=剪刀

    public string search_member(Gamer_data who)
        {
            for (int i = 0; i < 50; i++)
            {
                if (member[i].userid == who.userid)
                {
                    member[i]=getresult(member[i]);
                    return msg + "贏了" + member[i].win + "次," + "輸了" + member[i].lose + "次," + "平手" + member[i].draw + "次"; ;
                }
                if (member[i].userid == null)
                {
                    member[i] = getresult(who);
                    return msg + "贏了" + member[i].win + "次," + "輸了" + member[i].lose + "次," + "平手" + member[i].draw + "次"; ;
                }
            }

            return "好友人數已滿";
        }
       public Gamer_data getresult(Gamer_data who)
        {
            if (Gamer==0&&AI==1)
            {
                 msg+= "輸了,電腦出布,";
                who.lose++;
            }
            if (Gamer == 1 && AI == 0)
            {
                  msg += "贏了,電腦石頭,";
                who.win++;
            }
            if (Gamer == 0 && AI == 2)
            {
                  msg += "贏了,電腦出剪刀,";
                who.win++;
            }
            if (Gamer == 2 && AI == 0)
            {
                 msg += "輸了,電腦出石頭,";
                who.lose++;
            }
            if (Gamer == 1 && AI == 2)
            {
                 msg += "輸了,電腦出剪刀,";
                who.lose++;
            }
            if (Gamer == 2 && AI == 1)
            {
                 msg += "贏了,電腦出布,";
                who.win++;
            }
            if(Gamer ==  AI)
            {
                 msg += "平手,電腦和你一樣,";
                who.draw++;
            }
            return who;          
        }   
    }

    public class Ultimate
    {
        public int Gamer;//玩家猜的數字     
        public int win;
       // public int lose;
        public string msg;     
        public struct Gamer_data
        {
            public string userid;
           // public int lose;//玩家輸的次數
            public int Number;//電腦出的數字
            public int max;
            public int min;
        }
        public static Gamer_data[] member = new Gamer_data[50];
        public Gamer_data who = new Gamer_data();
        public Ultimate(string Userid, int gamer)
        {
            who.userid = Userid;
            Gamer = gamer;
            who.userid = Userid;
            Random random = new Random();
            who.Number = random.Next(0, 1001);
        }      
        //更新數字
        public string get_result(Gamer_data who)
        {
            string meg= "超過範圍";
            for (int i = 0; i < 50; i++)
            {
                if (member[i].userid == who.userid)
                {                 
                        if(Gamer < member[i].Number && Gamer>= member[i].min)
                        {                         
                            member[i].min = Gamer;
                            
                            meg = member[i].min.ToString() + "到" + member[i].max.ToString();
                        return meg;
                    }
                       else if(Gamer > member[i].Number && Gamer <= member[i].max)
                        {
                            member[i].max = Gamer;
                            
                           
                            meg = member[i].min.ToString() + "到" + member[i].max.ToString();
                        return meg;
                    }
                    else if (Gamer == member[i].Number)
                        {
                            member[i].max = 1000;
                            member[i].min = 0;                         
                            meg ="猜到數字"+ Gamer.ToString()+"爆炸";
                        Random random = new Random();
                        member[i].Number = random.Next(0, 1001);
                        return meg;
                        }
                    else
                    {                       
                        return meg;
                    }                   
                }
                if (member[i].userid == null)
                {
                       member[i] = who;
                       if (Gamer < member[i].Number)
                          {                      
                             member[i].min = Gamer;
                              meg = member[i].min.ToString() + "到" + member[i].max.ToString();
                              return meg;
                           }
                      else  if (Gamer > member[i].Number)
                       {
                           member[i].max = Gamer;
                           meg = member[i].min.ToString() + "到" + member[i].max.ToString();
                        return meg;
                    }
                     else  if (Gamer == member[i].Number)
                       {
                           member[i].max = 1000;
                           member[i].min = 0;
                        
                           meg = "猜到數字" + Gamer.ToString() + "爆炸";
                           Random random = new Random();
                           member[i].Number = random.Next(0, 1001);
                        return meg;
                    }
                    else
                    {
                        return meg;
                    }
                }
            }
            return meg;
        }
    }
}

