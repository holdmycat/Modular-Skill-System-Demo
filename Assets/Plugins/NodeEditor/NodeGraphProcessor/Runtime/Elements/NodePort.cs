// #define DEBUG_LAMBDA

using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Reflection;
using System.Linq.Expressions;
using System;

namespace GraphProcessor
{
	/// <summary>
	/// Class that describe port attributes for it's creation
	/// </summary>
	public class PortData : IEquatable< PortData >
	{
		/// <summary>
		/// Unique identifier for the port
		/// </summary>
		public string	identifier;
		/// <summary>
		/// Display name on the node
		/// </summary>
		public string	displayName;
		/// <summary>
		/// The type that will be used for coloring with the type stylesheet
		/// </summary>
		public Type		displayType;
		/// <summary>
		/// If the port accept multiple connection
		/// </summary>
		public bool		acceptMultipleEdges;
		/// <summary>
		/// Port size, will also affect the size of the connected edge
		/// </summary>
		public int		sizeInPixel;
		/// <summary>
		/// Tooltip of the port
		/// </summary>
		public string	tooltip;
		/// <summary>
		/// Is the port vertical
		/// </summary>
		public bool		vertical;
		/// <summary>
		/// Should this port draw an icon (default true). If using CustomPortBehavior, handle icons there.
		/// </summary>
		public bool showPortIcon = true;
		/// <summary>
		/// Optional type name to use for icon lookup; defaults to displayType when empty.
		/// </summary>
		/// <returns></returns>
		public string portIconName = null;

        public bool Equals(PortData other)
        {
			return identifier == other.identifier
				&& displayName == other.displayName
				&& displayType == other.displayType
				&& acceptMultipleEdges == other.acceptMultipleEdges
				&& sizeInPixel == other.sizeInPixel
				&& tooltip == other.tooltip
				&& vertical == other.vertical
				&& showPortIcon == other.showPortIcon
				&& portIconName == other.portIconName;
        }

		public void CopyFrom(PortData other)
		{
			identifier = other.identifier;
			displayName = other.displayName;
			displayType = other.displayType;
			acceptMultipleEdges = other.acceptMultipleEdges;
			sizeInPixel = other.sizeInPixel;
			tooltip = other.tooltip;
			vertical = other.vertical;
			showPortIcon = other.showPortIcon;
			portIconName = other.portIconName;
		}
    }

	/// <summary>
	/// Runtime class that stores all info about one port that is needed for the processing
	/// </summary>
	public class NodePort
	{
		/// <summary>
		/// The actual name of the property behind the port (must be exact, it is used for Reflection)
		/// </summary>
		public string				fieldName;
		/// <summary>
		/// The node on which the port is
		/// </summary>
		public BaseNode				owner;
		/// <summary>
		/// The fieldInfo from the fieldName
		/// Reflection info for the backing field on the node.
		/// </summary>
		public FieldInfo			fieldInfo;
		/// <summary>
		/// Data of the port
		/// </summary>
		public PortData				portData;
		List< SerializableEdge >	edges = new List< SerializableEdge >();
		public bool connected => this.GetEdges().Count > 0;

		/// <summary>
		/// Delegate that is made to send the data from this port to another port connected through an edge
		/// This is an optimization compared to dynamically setting values using Reflection (which is really slow)
		/// More info: https://codeblog.jonskeet.uk/2008/08/09/making-reflection-fly-and-exploring-delegates/
		/// </summary>
		public delegate void PushDataDelegate();
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="owner">owner node</param>
		/// <param name="fieldName">the C# property name</param>
		/// <param name="portData">Data of the port</param>
		public NodePort(BaseNode owner, string fieldName, PortData portData)
		{
			this.fieldName = fieldName;
			this.owner     = owner;
			this.portData  = portData;

			fieldInfo = owner.GetType().GetField(
				fieldName,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
		}

		/// <summary>
		/// Connect an edge to this port
		/// </summary>
		/// <param name="edge"></param>
		public void Add(SerializableEdge edge)
		{
			if (!edges.Contains(edge))
				edges.Add(edge);
		}

		/// <summary>
		/// Disconnect an Edge from this port
		/// </summary>
		/// <param name="edge"></param>
		public void Remove(SerializableEdge edge)
		{
			if (!edges.Contains(edge))
				return;
			
			edges.Remove(edge);
		}

		/// <summary>
		/// Get all the edges connected to this port
		/// </summary>
		/// <returns></returns>
		public List< SerializableEdge > GetEdges() => edges;

		/// <summary>
		/// Reset the value of the field to default if possible
		/// </summary>
		public void ResetToDefault()
		{
			// Clear lists, set classes to null and struct to default value.
			if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType))
				(fieldInfo.GetValue(owner) as IList)?.Clear();
			else if (fieldInfo.FieldType.GetTypeInfo().IsClass)
				fieldInfo.SetValue(owner, null);
			else
			{
				try
				{
					fieldInfo.SetValue(owner, Activator.CreateInstance(fieldInfo.FieldType));
				} catch {} // Catch types that don't have any constructors
			}
		}

		public void GetInputValue<T>(ref T value)
		{
			if (connected)
			{
				GetEdges()[0].outputPort.GetOutputValue(this, ref value);
			}
		}

		public void GetOutputValue<T>(NodePort inputPort, ref T value)
		{
			owner.TryGetOutputValue(this, inputPort, ref value);
		}
	}

	/// <summary>
	/// Container of ports and their connected edges. Can represent multiple ports for a node.
	/// </summary>
	public abstract class NodePortContainer : List< NodePort >
	{
		protected BaseNode node;

		public NodePortContainer(BaseNode node)
		{
			this.node = node;
		}

		/// <summary>
		/// Remove an edge that is connected to one of the node in the container
		/// </summary>
		/// <param name="edge"></param>
		public void Remove(SerializableEdge edge)
		{
			ForEach(p => p.Remove(edge));
		}

		/// <summary>
		/// Add an edge that is connected to one of the node in the container
		/// </summary>
		/// <param name="edge"></param>
		public void Add(SerializableEdge edge)
		{
			string portFieldName = (edge.inputNode == node) ? edge.inputFieldName : edge.outputFieldName;
			string portIdentifier = (edge.inputNode == node) ? edge.inputPortIdentifier : edge.outputPortIdentifier;

			// Force empty string to null since portIdentifier is a serialized value
			if (String.IsNullOrEmpty(portIdentifier))
				portIdentifier = null;

			var port = this.FirstOrDefault(p =>
			{
				return p.fieldName == portFieldName && p.portData.identifier == portIdentifier;
			});

			if (port == null)
			{
				Debug.LogError("The edge can't be properly connected because it's ports can't be found");
				return;
			}

			port.Add(edge);
		}
	}

	/// <inheritdoc/>
	public class NodeInputPortContainer : NodePortContainer
	{
		public NodeInputPortContainer(BaseNode node) : base(node) {}
	}

	/// <inheritdoc/>
	public class NodeOutputPortContainer : NodePortContainer
	{
		public NodeOutputPortContainer(BaseNode node) : base(node) {}
	}
}
