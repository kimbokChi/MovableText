﻿using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class MessageEditor : EditorWindow
{
    private const int ODD = 1;

    private float mVibration = 1f;
    private float mRotation  = 5f;
    private float mLetterSpacing = 20f;
    
    private uint mWaitFrame =  6;
    private  int mFontSize  = 26;

    private string mName;
    private string mMessage;
    
    private Canvas  mCanvas;
    private Vector3 mPosition;

    private Font  mFont;
    private Color mColor = Color.white;

    private FontStyle mFontStyle = FontStyle.Normal;
    private UnstableStyle mUnstable = UnstableStyle.Rotation;

    [MenuItem("Tools/Unstable Text/Create")]
    private static void Init()
    {
        MessageEditor window = EditorWindow.GetWindow(typeof(MessageEditor)) as MessageEditor;

        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Object Name", EditorStyles.label);
        mName = GUILayout.TextField(mName);

        GUILayout.Label("Message", EditorStyles.label);
        mMessage = EditorGUI.TextField(new Rect(2.5f, 55f, EditorGUIUtility.currentViewWidth - 7f, 18f), mMessage);

        GUILayout.Space(21f);

        GUILayout.Label("Font", EditorStyles.label);
        mFont = (Font)EditorGUILayout.ObjectField(mFont, typeof(Font), true);

        GUILayout.Label("Parent Canvas", EditorStyles.label);
        mCanvas = (Canvas)EditorGUILayout.ObjectField(mCanvas, typeof(Canvas), true);

        GUILayout.Space(16f);

        GUILayout.Label("Letter Spacing", EditorStyles.label);
        mLetterSpacing = EditorGUILayout.FloatField(mLetterSpacing);

        GUILayout.Label("Font Size", EditorStyles.label);
        mFontSize = EditorGUILayout.IntField(mFontSize);

        mFontStyle = (FontStyle)EditorGUILayout.EnumPopup("Font Style", mFontStyle);
        mUnstable = (UnstableStyle)EditorGUILayout.EnumPopup("Unstable Style", mUnstable);

        GUILayout.Label("Vibration", EditorStyles.label);
        mVibration = EditorGUILayout.Slider(mVibration, 0.01f, 3f);

        GUILayout.Label("Rotation", EditorStyles.label);
        mRotation = EditorGUILayout.Slider(mRotation, 0f, 180f);

        GUILayout.Label("Wait Frame", EditorStyles.label);
        mWaitFrame = (uint)EditorGUILayout.IntSlider((int)mWaitFrame, 0, 60);

        mPosition = EditorGUILayout.Vector3Field("Position", mPosition);

        GUILayout.Label("Color", EditorStyles.label);
        mColor = EditorGUILayout.ColorField(mColor);

        if (GUILayout.Button("Create!") && !EditorApplication.isPlaying) {
            Create();
        }
    }
    private void Create()
    {
        GameObject newObject = new GameObject(mName, typeof(RectTransform), typeof(UnstableText));

        Undo.RegisterCreatedObjectUndo(newObject, mName);

        newObject.transform.parent = mCanvas.transform;
        newObject.transform.localPosition = mPosition;

        if (newObject.TryGetComponent(out UnstableText text)) {
            text.Setting(mMessage, mLetterSpacing, 0f);
        }
        float charOffset = (mMessage.Length & ODD).Equals(ODD) ? 0f : mLetterSpacing * 0.5f;

        for (int i = 0; i < mMessage.Length; i++)
        {
            GameObject createChar = CreateUnStableChar(i, mMessage[i]);

            createChar.transform.parent = newObject.transform;
            createChar.transform.localPosition = new Vector2((-mMessage.Length / 2 + i) * mLetterSpacing + charOffset, 0);
        }
    }
    private GameObject CreateUnStableChar(int index, char letter)
    {
        string name = $"Character[{index}]";

        GameObject newObject = new GameObject(name, typeof(RectTransform), typeof(Text), typeof(UnstableObject));

        Undo.RegisterCreatedObjectUndo(newObject, name);

        if (newObject.TryGetComponent(out Text text)) 
        {
            text.text = letter.ToString();

            text.alignment = TextAnchor.MiddleCenter;

            text.fontSize = mFontSize; text.font = mFont;

            text.color = mColor; 
            
            text.fontStyle = mFontStyle;
        }
        if (newObject.TryGetComponent(out UnstableObject unstable)) 
        {
            unstable.Setting(mWaitFrame, mVibration, mRotation, mUnstable);
        }
        return newObject;
    }
}
