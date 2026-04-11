using System.Xml.Linq;
using SurgeEngine.Source.Code.Tools;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects
{
    public class SetRigidBody : StageObject, IHE1Importable
    {
        [SerializeField] private bool defaultOn = true;
        
        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.enabled = defaultOn;
        }
        
        public void Enable(bool value)
        {
            _collider.enabled = value;
        }

        public void ImportSetData(string objectName, XElement elem)
        {
            var box = GetComponent<BoxCollider>();
            float w = HE1Helper.GetFloat(elem, "Collision_Width");
            float h = HE1Helper.GetFloat(elem, "Collision_Height");
            float l = HE1Helper.GetFloat(elem, "Collision_Length");
            box.size = new Vector3(w, h, l);

            defaultOn = bool.Parse(elem.Element("DefaultON")!.Value.Trim());
        }
    }
}