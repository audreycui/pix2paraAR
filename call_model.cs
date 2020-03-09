using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Drawing;
using System.ComponentModel;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using System.Text;
using System.Threading.Tasks;

public class CallModel : MonoBehaviour
{

    public Button ScreenshotButton;

    // Start is called before the first frame update
    void Start()
    {
        ScreenshotButton.onClick.AddListener(takeScreenshot);
    }

    public void takeScreenshot()
    {
        //Camera myCamera = GameObject.GetComponent<ARCamera>(); 
        Texture2D tex = new Texture2D(Screen.width, Screen.height);
        tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        byte[] byteArray = tex.EncodeToPNG();
        Debug.Log("len: " + byteArray.GetLength(0));
        File.WriteAllBytes(Application.dataPath + "/screenshot.png", byteArray);
        //Color[] framebuffer = tex.GetPixels();
        //Debug.Log(framebuffer.GetLength(0));

   
        //ScreenCapture.CaptureScreenshot("screenshot.png");
        //System.Drawing.Bitmap image = new Bitmap(Application.dataPath + "screenshot.png");
        //byte[] imageArray = convertImageToFloat(image); 

        VGGProgram.InvokeRequestResponseService(byteArray);
        //tex.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static byte[] convertImageToFloat(Bitmap image)
    {
        MemoryStream stream = new MemoryStream();
        image.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
        byte[] bytes = stream.ToArray();
        Debug.Log(bytes); 
        //bytes = BitConverter.ToInt32(bytes, 0); 
        return bytes;
    }


}

class VGGStringTable
{
    [JsonProperty("data")]
    internal byte[] ImageArray { get; set; }
    // public string[,] Values { get; set; }
}


class VGGProgram
{

    public static async Task InvokeRequestResponseService(byte [] img)
    {


        string output = "hi";
        using (var client = new HttpClient())
        {

            /*
            var scoreRequest = new
            {

                Inputs = new VGGStringTable()
                {
                    ImageArray = img
                },
                GlobalParameters = new Dictionary<string, string>()
                {
                }
            };*/


            //const string apiKey = "hello";
            //const string apiKey = "PlrAUe05hxV951x++he0bqyUyCnOQ7Zx3cQqNwWWB6P111PFuXvMQQjdR1cNAo6PY7H3b3SaDrxLcwsVs5/2rg=="; // Replace this with the API key for the web service
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

             
            client.BaseAddress = new Uri("http://44ebd92f-f3e4-4524-a50a-b3569a653cd2.westus.azurecontainer.io/score");

            
            var scoreJson = JsonConvert.SerializeObject(img);
            //Debug.Log("here!");
            //File.WriteAllText(@"/screenshot_json.txt", scoreJson);
            //Debug.Log("here!");
            Debug.Log("scoreRequest: " + scoreJson);
            var content = new StringContent(scoreJson, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("", content);

           
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                Debug.Log("Result: " + result);
                output = result;
            }
            else
            {
                Debug.Log(string.Format("The request failed with status code: {0}", response.StatusCode));

                // Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
                Debug.Log(response.Headers.ToString());

                string responseContent = await response.Content.ReadAsStringAsync();
                Debug.Log(responseContent);
                output = "failed :(";
            }

            Debug.Log("output: " + output);
        }

    }
}

