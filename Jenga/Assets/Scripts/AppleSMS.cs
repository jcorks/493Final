using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class AppleSMS : MonoBehaviour {
	
   public struct sms_data
   {
      public int x, y, z;
   }
   
   sms_data myData;
   [DllImport ("AppleSMS")]
   private static extern sms_data getSMSValues();
   
   void OnGUI () {
      GUI.Box (new Rect (5,5,100,90), "Motion Sensor");
      myData = getSMSValues();
      GUI.Label (new Rect (20,25,100,50), "X: " + myData.x.ToString());
      GUI.Label (new Rect (20,40,100,50), "Y: " + myData.y.ToString());
      GUI.Label (new Rect (20,55,100,50), "Z: " + myData.z.ToString());
   }
}