using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;



namespace yazlab
{
    public class ChatHub : Hub
    {
        static List<UserDetail> BagliKullanicilar = new List<UserDetail>(); //Bağlı kullanıcıların bilgi listesi
        static List<MessageDetail> AnlikMesaj = new List<MessageDetail>(); //Anlik mesajların listesi

        //Server baglantisi
        public void Baglan(string kullanici ,string src)
        {

            var id = Context.ConnectionId;

            if (BagliKullanicilar.Count(x => x.ConnectionId == id) == 0)
            {

                BagliKullanicilar.Add(new UserDetail { ConnectionId = id, UserName = kullanici, Route = src });

                Clients.Caller.baglantiOlustu(id, kullanici, BagliKullanicilar, AnlikMesaj);

                Clients.AllExcept(id).yeniKullaniciGirisi(id, kullanici, src);

            }

        }
        //Mesaj Gonder Server kısmı
        public void MesajGonder(string iletilenID, string mesaj)
        {

            string gonderenID = Context.ConnectionId;

            var iletilen = BagliKullanicilar.FirstOrDefault(x => x.ConnectionId == iletilenID);
            var gonderen = BagliKullanicilar.FirstOrDefault(x => x.ConnectionId == gonderenID);

            if (iletilen != null && gonderen != null)
            {
                Clients.Client(iletilenID).mesajGonder(gonderenID, gonderen.UserName, mesaj, gonderen.Route);

                Clients.Caller.mesajGonder(iletilenID, gonderen.UserName, mesaj, iletilen.Route);
            }

        }

        //Bağlantı Kesme Server kısmı
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {

            var item = BagliKullanicilar.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                BagliKullanicilar.Remove(item);

                var id = Context.ConnectionId;
                Clients.All.baglantiKesme(id, item.UserName);

            }
            return base.OnDisconnected(stopCalled);
        }

        //Oturum kapama butonu server kısmı
        public void baglantiKes()
        {

            var item = BagliKullanicilar.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            BagliKullanicilar.Remove(item);
            var id = Context.ConnectionId;
            Clients.All.baglantiKesme(id, item.UserName);
        }
        
    }
}