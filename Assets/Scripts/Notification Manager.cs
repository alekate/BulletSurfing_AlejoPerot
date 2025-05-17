using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
#if UNITY_ANDROID
    private static string CHANNEL_ID = "Notifications01";

    private void Start()
    {
        //Creo los Notification Channels, una única vez.
        string NotiChannels_Created_Key = "NotiChannels_Created";
        if (!PlayerPrefs.HasKey(NotiChannels_Created_Key))
        {
            var group = new AndroidNotificationChannelGroup()
            {
                Id = "Main", //Mismo nomnbre que el Channel que va a controlar
                Name = "Main notifications",
            };
            AndroidNotificationCenter.RegisterNotificationChannelGroup(group);

            var channel = new AndroidNotificationChannel()
            {
                Id = CHANNEL_ID,
                Name = "Default Channel",
                Importance = Importance.Default,
                Description = "Generic notifications",
                Group = "Main",  // Tiene que ser el mismo Id del grupo que creé antes
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);

            StartCoroutine(RequestPermission()); //Coorutina para esperar la validacion del jugador para las Push Notifications

            PlayerPrefs.SetString(NotiChannels_Created_Key, "y"); //Guardo una "Coockie" que valida si ya se creo o no.
            PlayerPrefs.Save();
        }
        else
        {
            ScheduleNotifications();
        }
    }

    private IEnumerator RequestPermission()
    {
        var request = new PermissionRequest();
        while (request.Status == PermissionStatus.RequestPending)
            yield return null;

        ScheduleNotifications();
    }

    private void ScheduleNotifications() //Funcion que maneja el Pusheo de notificaciones
    {
        //Elimino las notis que había creado en la sesión anterior.
        AndroidNotificationCenter.CancelAllScheduledNotifications();

        //Y las creo de nuevo:
        var notificationsWelcome = new AndroidNotification();
        notificationsWelcome.Title = "Hi!";
        notificationsWelcome.Text = "Thanks for downloading <3 I hope you enjoy my first mobile game!";
        notificationsWelcome.FireTime = System.DateTime.Now.AddMinutes(1);

        AndroidNotificationCenter.SendNotification(notificationsWelcome, CHANNEL_ID); //Envia la notificacion

        var notification10Minutes = new AndroidNotification();
        notification10Minutes.Title = "WOW, you have played a lot!";
        notification10Minutes.Text = "I think it's time for presentations don't you? My name is Alejo Perot nice to meet you";
        notification10Minutes.FireTime = System.DateTime.Now.AddMinutes(10);

        AndroidNotificationCenter.SendNotification(notification10Minutes, CHANNEL_ID);

        /*
        var notification7days = new AndroidNotification();
        notification7days.Title = "Hola, ¿estas ahi?";
        notification7days.Text = "Hace siete días que no abris mi juego, ¿me enojo?";
        notification7days.FireTime = System.DateTime.Now.AddDays(7);

        AndroidNotificationCenter.SendNotification(notification7days, CHANNEL_ID);
        */
    }
#endif
}
