using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ShrugWare
{
    public class TooltipManager : MonoBehaviour
    {
        public static TooltipManager Instance;

        [SerializeField]
        TextMeshProUGUI textComponent;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        void Update()
        {
            transform.position = new Vector3(Input.mousePosition.x + 200, Input.mousePosition.y + 30, Input.mousePosition.z);
        }

        public void SetAndShowTooltip(string tooltipMessage)
        {
            gameObject.SetActive(true);
            textComponent.text = tooltipMessage;
            Cursor.visible = false;
        }

        public void HideTooltip()
        {
            gameObject.SetActive(false);
            textComponent.text = "";
            Cursor.visible = true;
        }
    }
}