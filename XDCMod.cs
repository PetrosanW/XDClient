using MelonLoader;
using UnityEngine;
using XDClient.Feature;

namespace XDClient
{
    // ReSharper disable once InconsistentNaming
    public class XDCMod : MelonMod
    {
        private static bool _showMenu;
        private static bool _espEnabled;

        private static GUIStyle? _mainBoxStyle;
        private static GUIStyle? _labelStyle;
        private static GUIStyle? _headerStyle;
        private static GUIStyle? _toggleStyle;
        private static GUIStyle? _buttonStyle;
        private static GUIStyle? _tabButtonStyle;
        private static GUIStyle? _activeTabStyle;

        private readonly PointsEditor _pointsEditor = new();
        private readonly EspOverlay _espOverlay = new();
        
        private enum Tab
        {
            Main,
            PointEditor
        }

        private Tab _currentTab = Tab.Main;

        private Rect _windowPosition = new Rect(10, 10, 400, 300);
        
        private bool _isDragging;
        private Vector2 _dragOffset;

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("[XDClient] Started");
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                _showMenu = !_showMenu;
            }
        }

        public override void OnGUI()
        {
            if (_espEnabled)
            {
                _espOverlay.Render();
            }

            RenderMenu();
        }

        private static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i) pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        private void RenderMenu()
        {
            if (!_showMenu) return;
            
            if (_mainBoxStyle == null)
            {
                _mainBoxStyle = new GUIStyle(GUI.skin.box);
                _mainBoxStyle.normal.background = MakeTex(2, 2, new Color(0f, 0f, 0f, 0.8f));
            }

            if (_labelStyle == null)
            {
                _labelStyle = new GUIStyle(GUI.skin.label);
                _labelStyle.fontSize = 16;
                _labelStyle.normal.textColor = Color.white;
            }

            if (_buttonStyle == null)
            {
                _buttonStyle = new GUIStyle(GUI.skin.button);
                _buttonStyle.fontSize = 16;
            }

            if (_toggleStyle == null)
            {
                _toggleStyle = new GUIStyle(GUI.skin.toggle);
                _toggleStyle.onNormal.textColor = Color.white;
                _toggleStyle.fontSize = 16;
            }

            if (_tabButtonStyle == null)
            {
                _tabButtonStyle = new GUIStyle(GUI.skin.button);
                _tabButtonStyle.fontSize = 14;
                _tabButtonStyle.fixedWidth = 120;
                _tabButtonStyle.fixedHeight = 35;
            }

            if (_activeTabStyle == null)
            {
                _activeTabStyle = new GUIStyle(_tabButtonStyle);
                _activeTabStyle.normal.textColor = Color.white;
            }
            
            GUI.BeginGroup(_windowPosition, "", _mainBoxStyle);
            
            DrawHud();
            
            HandleDrag(new Rect(0, 0, _windowPosition.width, 30));
            
            GUILayout.BeginArea(new Rect(10, 40, _windowPosition.width - 20, 40));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Main", _currentTab == Tab.Main ? _activeTabStyle : _tabButtonStyle))
            {
                _currentTab = Tab.Main;
            }

            GUILayout.Space(10);

            if (GUILayout.Button("PointEditor", _currentTab == Tab.PointEditor ? _activeTabStyle : _tabButtonStyle))
            {
                _currentTab = Tab.PointEditor;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            
            switch (_currentTab)
            {
                case Tab.Main:
                    DrawMainTab(new Rect(10, 90, _windowPosition.width - 20, _windowPosition.height - 100));
                    break;
                case Tab.PointEditor:
                    DrawPointEditorTab(new Rect(10, 90, _windowPosition.width - 20, _windowPosition.height - 100));
                    break;
            }

            GUI.EndGroup();
        }

        private void DrawHud()
        {
            if (_headerStyle == null)
            {
                _headerStyle = new GUIStyle(GUI.skin.box);
                _headerStyle.normal.background = MakeTex(2, 2, Color.black);
                _headerStyle.normal.textColor = Color.white;
                _headerStyle.fontSize = 16;
                _headerStyle.alignment = TextAnchor.MiddleCenter;
            }
            
            GUI.Box(new Rect(0, 0, _windowPosition.width, 30), $"XDClient | by Petrosan", _headerStyle);
        }

        private void HandleDrag(Rect headerRect)
        {
            Event e = Event.current;
            Vector2 mousePosition = e.mousePosition;

            if (e.type == EventType.MouseDown && e.button == 0 && headerRect.Contains(mousePosition))
            {
                _isDragging = true;
                _dragOffset = mousePosition;
                e.Use();
            }

            if (e.type == EventType.MouseUp && e.button == 0)
            {
                _isDragging = false;
            }

            if (_isDragging && e.type == EventType.MouseDrag && e.button == 0)
            {
                _windowPosition.x += e.delta.x;
                _windowPosition.y += e.delta.y;
                
                _windowPosition.x = Mathf.Clamp(_windowPosition.x, 0, Screen.width - _windowPosition.width);
                _windowPosition.y = Mathf.Clamp(_windowPosition.y, 0, Screen.height - _windowPosition.height);

                e.Use();
            }
        }

        private void DrawMainTab(Rect contentRect)
        {
            GUILayout.BeginArea(contentRect);
            GUILayout.BeginVertical();
            
            _espEnabled = GUILayout.Toggle(_espEnabled, " ESP", _toggleStyle);

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void DrawPointEditorTab(Rect contentRect)
        {
            GUILayout.BeginArea(contentRect);
            GUILayout.BeginVertical();

            if (GUILayout.Button(" +500", _buttonStyle))
            {
                _pointsEditor.AddPoints(1000);
            }
            
            if (GUILayout.Button(" +1000", _buttonStyle))
            {
                _pointsEditor.AddPoints(1000);
            }

            GUILayout.Space(10);

            if (GUILayout.Button(" -500", _buttonStyle))
            {
                _pointsEditor.RemovePoints(500);
            }

            if (GUILayout.Button(" -1000", _buttonStyle))
            {
                _pointsEditor.RemovePoints(1000);
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}
