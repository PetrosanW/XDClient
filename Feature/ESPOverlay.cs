using Il2CppBrokenArrow.DataBase.Models;
using Il2CppBrokenArrow.ScriptEngine.Nodes.Economy;
using UnityEngine;

namespace XDClient.Feature
{
    public class EspOverlay
    {
        NodeGetMoney.
        
        private static Material? _lineMaterial;
        private GUIStyle? _style;
        private readonly List<LabelInfo> _labels = new();
        
        private struct LabelInfo
        {
            public Vector3 Position;
            public string Text;
        }
        
        public void Render()
        {
            _style ??= new GUIStyle();
            _style.normal.textColor = Color.red;
            _style.fontStyle = FontStyle.Bold;
            _style.alignment = TextAnchor.MiddleCenter;
            _style.fontSize = 14;
            
            foreach (var label in _labels)
            {
                if (Camera.main == null)
                {
                    return;
                }
                
                Vector3 screenPos = Camera.main.WorldToScreenPoint(label.Position);
                
                if (screenPos.z > 0)
                {
                    screenPos.y = Screen.height - screenPos.y;
                    
                    GUI.Label(new Rect(screenPos.x - 50, screenPos.y - 20, 100, 20), label.Text, _style);
                }
            }
            
            DrawEntities();
        }
    
        private void DrawEntities()
        {
            _labels.Clear();
            
            GameObject entityHub = GameObject.Find("UNITS HUB");
        
            if (entityHub == null)
            {
                return;
            }
    
            Transform hubTransform = entityHub.transform;
            int childCount = hubTransform.childCount;
            
            int deadUnitsLayer = LayerMask.NameToLayer("DeadUnits");
    
            for (int i = 0; i < childCount; i++)
            {
                Transform child = hubTransform.GetChild(i);
                
                if (deadUnitsLayer != -1 && child.gameObject.layer == deadUnitsLayer)
                {
                    continue;
                }
                
                BoxCollider boxCollider = child.GetComponent<BoxCollider>();
                
                if (boxCollider != null)
                {
                    // Рисуем рамку вокруг BoxCollider
                    DrawBoxCollider(boxCollider);
                    
                    Vector3 topCenter = boxCollider.transform.TransformPoint(boxCollider.center + new Vector3(0, boxCollider.size.y / 2, 0));
                    
                    _labels.Add(new LabelInfo
                    {
                        Position = topCenter,
                        Text = child.name
                    });
                }
            }
        }
        
        private void DrawBoxCollider(BoxCollider boxCollider)
        {
            Vector3 center = boxCollider.transform.TransformPoint(boxCollider.center);
            Vector3 size = Vector3.Scale(boxCollider.size, boxCollider.transform.lossyScale);
            Quaternion rotation = boxCollider.transform.rotation;
    
            Vector3 extents = size / 2;
            
            Vector3[] vertices = new Vector3[8];
    
            vertices[0] = center + rotation * new Vector3(-extents.x, -extents.y, -extents.z);
            vertices[1] = center + rotation * new Vector3(extents.x, -extents.y, -extents.z);
            vertices[2] = center + rotation * new Vector3(extents.x, -extents.y, extents.z);
            vertices[3] = center + rotation * new Vector3(-extents.x, -extents.y, extents.z);
            vertices[4] = center + rotation * new Vector3(-extents.x, extents.y, -extents.z);
            vertices[5] = center + rotation * new Vector3(extents.x, extents.y, -extents.z);
            vertices[6] = center + rotation * new Vector3(extents.x, extents.y, extents.z);
            vertices[7] = center + rotation * new Vector3(-extents.x, extents.y, extents.z);
            
            DrawLine(vertices[0], vertices[1]);
            DrawLine(vertices[1], vertices[2]);
            DrawLine(vertices[2], vertices[3]);
            DrawLine(vertices[3], vertices[0]);
    
            DrawLine(vertices[4], vertices[5]);
            DrawLine(vertices[5], vertices[6]);
            DrawLine(vertices[6], vertices[7]);
            DrawLine(vertices[7], vertices[4]);
    
            DrawLine(vertices[0], vertices[4]);
            DrawLine(vertices[1], vertices[5]);
            DrawLine(vertices[2], vertices[6]);
            DrawLine(vertices[3], vertices[7]);
        }
        
        private void DrawLine(Vector3 start, Vector3 end)
        {
            if (Camera.main == null)
            {
                return;
            }
            
            if (_lineMaterial == null)
            {
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                _lineMaterial = new Material(shader)
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                _lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                _lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                _lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                _lineMaterial.SetInt("_ZWrite", 0);
            }
    
            _lineMaterial.SetPass(0);
    
            GL.PushMatrix();
            GL.LoadProjectionMatrix(Camera.main.projectionMatrix);
            GL.modelview = Camera.main.worldToCameraMatrix;
    
            GL.Begin(GL.LINES);
            GL.Color(Color.red);
            GL.Vertex(start);
            GL.Vertex(end);
            GL.End();
    
            GL.PopMatrix();
        }
    }
}
