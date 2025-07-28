using System.Collections;
#if !UNITY_WEBGL
using Unity.Notifications.Android;
#endif
using UnityEngine;

public class NotificationsManager : MonoBehaviour {
#pragma warning disable 0414
    private static string CHANNEL_ID = "notis";
#pragma warning restore 0414

#if !UNITY_WEBGL
    void Start() {
        if (!PlayerPrefs.HasKey("NotisChanel_Created")) {
            // Create the notification group
            var group = new AndroidNotificationChannelGroup() {
                Id = "Main",
                Name = "Main Notifications"
            };
            AndroidNotificationCenter.RegisterNotificationChannelGroup(group);

            // Create the notification channel
            var channel = new AndroidNotificationChannel() {
                Id = CHANNEL_ID,
                Name = "Default Channel",
                Importance = Importance.Default,
                Description = "Main notifications for the app.",
                Group = group.Id
            };
            AndroidNotificationCenter.RegisterNotificationChannel(channel);

            StartCoroutine(RequestNotificationsPermission());

            // Set the PlayerPrefs key to indicate that the channel has been created
            PlayerPrefs.SetInt("NotisChanel_Created", 1);
            PlayerPrefs.Save();
        }
        else {
            // Schedule the notification
            ScheduleNotification();
        }
    }

    private IEnumerator RequestNotificationsPermission() {
        // Request permission to send notifications
        var request = new PermissionRequest();

        while (request.Status == PermissionStatus.RequestPending)
            yield return new WaitForEndOfFrame();

        // Schedule notifications if permission is granted
        ScheduleNotification();
    }

    private void ScheduleNotification() {
        // Cancel all previous notifications
        AndroidNotificationCenter.CancelAllScheduledNotifications();

        // Create a new notification
        var notification10Minutes = new AndroidNotification() {
            Title = "Final Portabilida y optimización",
            Text = "Juego creado por Daniel Fimiani",
            FireTime = System.DateTime.Now.AddMinutes(10),
        };

        // Schedule the notification
        AndroidNotificationCenter.SendNotification(notification10Minutes, CHANNEL_ID);
        Debug.Log("Notification scheduled for 10 minutes from now.");
    }
#endif
}
