using System;
using System.Text;
using HslCommunication.Core.Net;
using HslCommunication.LogNet;
using HslCommunication.MQTT;

namespace HslCommunication.Enthernet.Tcp;

internal class VpnPortServer : CommunicationServer
{
	private MqttSession mqttSession = null;

	private MqttServer mqttServer = null;

	private ILogNet logNet = null;

	public void SetMqtt(MqttServer mqttServer, MqttSession mqttSession, ILogNet logNet)
	{
		this.mqttSession = mqttSession;
		this.mqttServer = mqttServer;
		this.logNet = logNet;
		base.OnClientOnline += Server_OnClientOnline;
		base.OnClientOffline += Server_OnClientOffline;
		base.OnPipeMessageReceived += Server_OnPipeMessageReceived;
	}

	private void Server_OnClientOnline(object server, PipeSession session)
	{
		session.SessionID = Guid.NewGuid().ToString("N");
		mqttServer.PublishTopicPayload(mqttSession, "Online", Encoding.UTF8.GetBytes(session.SessionID));
		logNet?.WriteInfo(mqttSession.ToString() + "   Client: " + session.ToString() + " online");
	}

	private void Server_OnClientOffline(object server, PipeSession session)
	{
		mqttServer.PublishTopicPayload(mqttSession, "Offline", Encoding.UTF8.GetBytes(session.SessionID));
		logNet?.WriteInfo(mqttSession.ToString() + "   Client: " + session.ToString() + " offline");
	}

	private void Server_OnPipeMessageReceived(PipeSession session, byte[] buffer)
	{
		mqttServer.PublishTopicPayload(mqttSession, "Data:" + session.SessionID, buffer);
	}
}
