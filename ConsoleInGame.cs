using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// A console that displays the contents of Unity's debug log.
/// </summary>
/// <remarks>
/// Developed by Matthew Miner (www.matthewminer.com)
/// Forked by Joseph Cassano (jplc.ca) from Console to ConsoleInGame so the console can work from other scripts.
/// Fork adds ExampleSceneScript.cs to show how to use this class in another script.
/// Permission is given to use this script however you please with absolutely no restrictions.
/// </remarks>
public static class ConsoleInGame
{
    public static readonly Version version = new Version(1, 0);

    struct ConsoleMessage
    {
        public readonly string message;
        public readonly string stackTrace;
        public readonly LogType type;

        public ConsoleMessage(string message, string stackTrace, LogType type)
        {
            this.message = message;
            this.stackTrace = stackTrace;
            this.type = type;
        }
    }

    static List<ConsoleMessage> entries = new List<ConsoleMessage>();
    static Vector2 scrollPos;
    static bool collapse;

    // Visual elements:

    static GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
    static GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");

    /// <summary>
    /// A window displaying the logged messages.
    /// </summary>
    /// <param name="windowID">The window's ID.</param>
    public static void ConsoleWindow(int windowID)
    {
        scrollPos = GUILayout.BeginScrollView(scrollPos);

        // Go through each logged entry
        for (int i = 0; i < entries.Count; i++)
        {
            ConsoleMessage entry = entries[i];

            // If this message is the same as the last one and the collapse feature is chosen, skip it
            if (collapse && i > 0 && entry.message == entries[i - 1].message)
            {
                continue;
            }

            // Change the text colour according to the log type
            switch (entry.type)
            {
                case LogType.Error:
                case LogType.Exception:
                    GUI.contentColor = Color.red;
                    break;

                case LogType.Warning:
                    GUI.contentColor = Color.yellow;
                    break;

                default:
                    GUI.contentColor = Color.white;
                    break;
            }

            GUILayout.Label(entry.message);
        }

        GUI.contentColor = Color.white;

        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();

        // Clear button
        if (GUILayout.Button(clearLabel))
        {
            entries.Clear();
        }

        // Collapse toggle
        collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));

        GUILayout.EndHorizontal();

        // Set the window to be draggable by the top title bar
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }

    /// <summary>
    /// Logged messages are sent through this callback function.
    /// </summary>
    /// <param name="message">The message itself.</param>
    /// <param name="stackTrace">A trace of where the message came from.</param>
    /// <param name="type">The type of message: error/exception, warning, or assert.</param>
    public static void HandleLog(string message, string stackTrace, LogType type)
    {
        ConsoleMessage entry = new ConsoleMessage(message, stackTrace, type);
        entries.Add(entry);
    }
}