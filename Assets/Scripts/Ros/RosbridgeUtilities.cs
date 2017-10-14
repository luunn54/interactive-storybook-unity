// Jacqueline Kory Westlund
// June 2016
//
// The MIT License (MIT)
// Copyright (c) 2016 Personal Robots Group
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;
using MiniJSON;
using UnityEngine;


public static class RosbridgeUtilities
{
	/// <summary>
	/// Builds a JSON string message to publish over rosbridge
	/// </summary>
	/// <returns>A JSON string to send</returns>
	/// <param name="topic">The topic to publish on </param>
	/// <param name="message">The string message to send</param>
	public static string GetROSJsonPublishStringMsg(string topic, string message)
	{
		// build a dictionary of things to include in the message
		Dictionary<string, object> rosPublish = new Dictionary<string, object>();
		rosPublish.Add("op", "publish");
		rosPublish.Add("topic", topic);

		Dictionary<string, object> rosMessage = new Dictionary<string, object>();
		rosMessage.Add("data", message);
		rosPublish.Add("msg", rosMessage);

		return Json.Serialize(rosPublish);
	}

	public static string GetROSJsonPublishLogMsg(string topic, string logMessage)
	{
		// build a dictionary of things to include in the message
		Dictionary<string, object> rosPublish = new Dictionary<string, object>();
		rosPublish.Add("op", "publish");
		rosPublish.Add("topic", topic);

		Dictionary<string, object> rosMessage = new Dictionary<string, object>();
		rosMessage.Add("message", logMessage);
		rosPublish.Add("msg", rosMessage);

		return Json.Serialize(rosPublish);
	}

	/// <summary>
	/// Build a JSON string message to subscribe to a rostopic over rosbridge
	/// </summary>
	/// <returns>A JSON string to send</returns>
	/// <param name="topic">The topic to subscribe to</param>
	/// <param name="messageType">The rosmsg type of the topic</param>
	public static string GetROSJsonSubscribeMsg(string topic, string messageType)
	{
		// build a dictionary of things to include in the message
		Dictionary<string, object> rosSubscribe = new Dictionary<string, object>();
		rosSubscribe.Add("op", "subscribe");
		rosSubscribe.Add("topic", topic);
		rosSubscribe.Add("type", messageType);

		return Json.Serialize(rosSubscribe);
	}


	/// <summary>
	/// Build a JSON string message to advertise a rostopic over rosbridge
	/// </summary>
	/// <returns>A JSON string to send</returns>
	/// <param name="topic">The topic to advertise</param>
	/// <param name="messageType">The rosmsg type of the topic</param>
	public static string GetROSJsonAdvertiseMsg(string topic, string messageType)
	{
		// build a dictionary of things to include in the message
		Dictionary<string, object> rosAdvertise = new Dictionary<string, object>();
		rosAdvertise.Add("op", "advertise");
		rosAdvertise.Add("topic", topic);
		rosAdvertise.Add("type", messageType);

		return Json.Serialize(rosAdvertise);
	}

	/// <summary>
	/// Decode a ROS JSON command message
	/// </summary>
	/// <param name="msg">the message received</param>
	/// <param name="command">the command received</param>
	/// <param name="properties">command properties received</param>
	public static void DecodeROSJsonCommand(string rosmsg, out int command,
										out object msgParams)
	{
		// set up out objects
		command = -1;
		msgParams = null;
		// there is also a header in the command message, but we aren't
		// using it for anything
		// 
		// parse data, see if it's valid
		//            
		// should be valid json, so we try parsing the json

		Dictionary<string, object> data = null;
		data = Json.Deserialize(rosmsg) as Dictionary<string, object>;
		if (data == null)
		{
			Logger.LogWarning("[decode ROS msg] Could not parse JSON message!");
			return;
		}
		Logger.Log("[decode ROS msg] deserialized " + data.Count + " objects from JSON!");

		// message sent over rosbridge comes with the topic name and what the
		// operation was
		//
		// TODO should we check that the topic matches one that we're subscribed
		// to before parsing further? Would need to keep a list of subscriptions. 



		// if the message doesn't have all three parts, consider it invalid
		if (!data.ContainsKey("msg") && !data.ContainsKey("topic")
			&& !data.ContainsKey("op"))
		{
			Logger.LogWarning("[decode ROS msg] Did not get a valid message!");
			return;
		}

		Logger.Log("[decode ROS msg] Got " + data["op"] + " message on topic " + data["topic"]);

		// parse the actual message
		Logger.Log("[decode ROS msg] Parsing message: " + data["msg"]);
		Dictionary<string, object> msg = data["msg"] as Dictionary<string, object>;

		// print header for debugging
		if (msg.ContainsKey("header"))
		{
			Logger.Log("[decode ROS msg]" + msg["header"]);
		}

		// get the command
		if (msg.ContainsKey("command"))
		{
			Logger.Log("[decode ROS msg] command: " + msg["command"]);
			try
			{
				command = Convert.ToInt32(msg["command"]);
			}
			catch (Exception ex)
			{
				Logger.LogError("[decode ROS msg] Error! Could not get command: " + ex);
			}
		}

		// if the params are missing or there aren't any params, 
		// we're done, return command only
		if (!msg.ContainsKey("params") ||
			((string)msg["params"]).Equals(""))
		{
			Logger.Log("[decode ROS msg] no params found, done parsing");
			return;
		}

		// otherwise, we've got params, decode them.
		Logger.Log("[decode ROS msg] params: " + msg["params"]);

		// parse data, see if it's valid json
		Dictionary<string, object> msgParamDict = null;
		Logger.Log("here t is as a string!");
		Logger.Log((string)msg["params"]);
		msgParamDict = Json.Deserialize((string)msg["params"]) as Dictionary<string, object>;
		// if we can't deserialize the json message, return
		if (msgParamDict == null)
		{
			Logger.Log("[decode ROS msg] Could not parse JSON properties! Could just be a string.");


			// params can be just a string (e.g. for loading animals)
			if (msg["params"] is String)
			{
				msgParams = (string)msg["params"];
			}
			else
			{
				Logger.LogWarning("[decode ROS msg] Could not parse as a string either!");
				msgParams = "";
			}
			return;
		}


		if (msgParamDict.ContainsKey("letters"))
		{
			Logger.Log("DecodeROSJson detected letters!");
			msgParams = msgParamDict;
		}
		// otherwise, we got properties!
		Logger.Log("[decode ROS msg] deserialized " + msgParamDict.Count + " properties from JSON!");
		Logger.Log(msgParamDict);

	}

	/// <summary>
	/// convert an object to an int array
	/// </summary>
	/// <returns>int array</returns>
	/// <param name="en">object that is secretly an int array</param>
	private static int[] ObjectToIntArray(IEnumerable en)
	{
		// C# is weird about conversions from object to arrays
		// so this is a hack way of converting an object into an
		// IEnumerable so we can then convert each element of the
		// array to a number, so we can then make an array.....
		int[] posn = { 0, 0, 0 };
		if (en != null)
		{
			int count = 0;
			foreach (object el in en)
			{
				posn[count] = Convert.ToInt32(el);
				count++;
			}
		}
		return posn;
	}

	/// <summary>
	/// convert object to a string array
	/// </summary>
	/// <returns>string array.</returns>
	/// <param name="en">object that is secretly a string array</param>
	private static string[] ObjectToStringArray(IEnumerable en)
	{
		// C# is weird about conversions from object to arrays
		// so this is a hack way of converting an object into an
		// IEnumerable so we can then convert each element of the
		// array to a string, so we can then make an array.....
		string[] s;
		if (en != null)
		{
			// get length of array
			int count = 0;
			foreach (object el in en)
			{
				count++;
			}
			// make a destination array of the right size 
			s = new string[count];

			// reset counter
			count = 0;

			// convert each element to a string
			foreach (object el in en)
			{
				s[count] = Convert.ToString(el);
				count++;
			}
			return s;
		}
		return null;
	}

	/// <summary>
	/// Get the ROS header.
	/// </summary>
	/// <returns>The ROS header.</returns>
	public static Dictionary<string, object> GetROSHeader()
	{
		Dictionary<string, object> header = new Dictionary<string, object>();
		// header sequence number (ROS overrides this)
		header.Add("seq", 0);
		// header frame (no frame)
		header.Add("frame_id", "");
		// time for header
		Dictionary<string, Int32> time = new Dictionary<string, Int32>();
		TimeSpan unixtime = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1));
		time.Add("sec", (Int32)(unixtime.TotalSeconds));
		time.Add("nsec", (Int32)(unixtime.Milliseconds * 1000));
		// add time to header
		header.Add("stamp", time);
		return header;
	}
}

