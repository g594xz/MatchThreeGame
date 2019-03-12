using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Gemstone : MonoBehaviour
{
    public int row = 0;
    public int column = 0;
    public float xOffset = -5.0f;
    public float yOffset = -3.0f;

    public GameObject[] gemstones;
    public int gemstoneKind;

    private GameObject gemstone;
    public MenuController menuController;

    public GameController gameController;
    public SpriteRenderer spriteRenderer;
    public bool isSelected
    {
        set
        {
            if (value)
            {
                spriteRenderer.color = Color.red;
;           }
            else
            {
                spriteRenderer.color = Color.white;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        spriteRenderer = gemstone.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GemStonePosition(int rowNum, int columnNum)
    {
        row = rowNum;
        column = columnNum;
        this.transform.position = new Vector3(column*1.1f + xOffset, row*1.1f + yOffset, 0);
    }

    public void TweenPosition(int rowNum, int columnNum)
    {
        row = rowNum;
        column = columnNum;
        iTween.MoveTo(this.gameObject, iTween.Hash("x", column * 1.1f + xOffset, "y", row * 1.1f + yOffset, "time", 0.5f));
    }

    public void GenerateRandomGemstone()
    {
        if (gemstone != null)
        {
            return;
        }
        gemstoneKind = Random.Range(0, gemstones.Length);
        gemstone = Instantiate(gemstones[gemstoneKind]) as GameObject;
        gemstone.transform.parent = this.transform;
    }

    public void OnMouseDown()
    {
        if (!IsPointerOverUIObject())
        {
            gameController.GetGemstone(this);

        }
    }

    public void GemstoneDispose()
    {
        Destroy(this.gameObject);
        Destroy(gemstone.gameObject);
        gameController = null;
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
