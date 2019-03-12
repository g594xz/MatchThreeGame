using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Gemstone gemstone;
    public int row = 7;
    public int column = 10;
    public ArrayList gemstoneList;
    private Gemstone selectedGemstone;
    private ArrayList sameGemstones;
    public AudioClip sameKind;
    public AudioClip changePosition;
    public AudioClip notSame;
    // Start is called before the first frame update
    void Start()
    {
        gemstoneList = new ArrayList();
        sameGemstones = new ArrayList();
        for (int i = 0; i < row; i++)
        {
            ArrayList colStones = new ArrayList();
            for (int j = 0; j < column; j++)
            {
                Gemstone gs = Instantiate(gemstone) as Gemstone;
                gs.transform.parent = this.transform;
                colStones.Add(gs);
                gs.GetComponent<Gemstone>().GenerateRandomGemstone();
                gs.GetComponent<Gemstone>().GemStonePosition(i, j);
                
            }
            gemstoneList.Add(colStones);
        }

        if (CheckKindHorizontally() || CheckKindVertically())
        {
            RemoveSameGemstones();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetGemstone(Gemstone gs)
    {
        if (selectedGemstone == null)
        {
            selectedGemstone = gs;
            selectedGemstone.isSelected = true;
            return;
        }
        else
        {
            if ( (Mathf.Abs(selectedGemstone.row - gs.row) + (Mathf.Abs(selectedGemstone.column - gs.column))) == 1)
            {
                StartCoroutine(ChangePositionAndDetectKind(selectedGemstone, gs));
            }
            else
            {
                AudioSource audio = GetComponent<AudioSource>();
                audio.PlayOneShot(notSame);
            }
            
            selectedGemstone.isSelected = false;
            selectedGemstone = null;
        }
    }

    public Gemstone AddGemstone(int row, int column)
    {
        Gemstone gs = Instantiate(gemstone) as Gemstone;
        gs.transform.parent = this.transform;
        gs.GetComponent<Gemstone>().GenerateRandomGemstone();
        gs.GetComponent<Gemstone>().GemStonePosition(row, column);
        return gs;
    }

    public void ChangeGemstonePosition(Gemstone gs1, Gemstone gs2)
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.PlayOneShot(changePosition);
        SetGemstonePosition(gs1.row, gs1.column, gs2);
        SetGemstonePosition(gs2.row, gs2.column, gs1);

        int tempRow;
        tempRow = gs1.row;
        gs1.row = gs2.row;
        gs2.row = tempRow;

        int tempCol;
        tempCol = gs1.column;
        gs1.column = gs2.column;
        gs2.column = tempCol;

        gs1.TweenPosition(gs1.row, gs1.column);
        gs2.TweenPosition(gs2.row, gs2.column);
    }

    public Gemstone GetGemstoneByPosition(int row, int column)
    {
        ArrayList temp = gemstoneList[row] as ArrayList;
        Gemstone c = temp[column] as Gemstone;
        return c;
    }

    public void SetGemstonePosition(int row, int column, Gemstone c)
    {
        ArrayList temp = gemstoneList[row] as ArrayList;
        temp[column] = c;
    }

    IEnumerator ChangePositionAndDetectKind(Gemstone gs1, Gemstone gs2)
    {
        ChangeGemstonePosition(gs1, gs2);
        yield return new WaitForSeconds(0.5f);
        if (CheckKindHorizontally() || CheckKindVertically())
        {
            RemoveSameGemstones();
        }
        else
        {
            ChangeGemstonePosition(gs1, gs2);
        }
    }

    public void SaveSameGemstone(Gemstone gs)
    {
        if (sameGemstones == null)
        {
            sameGemstones = new ArrayList();
        }
        int index = sameGemstones.IndexOf(gs);
        if (index == -1)
        {
            sameGemstones.Add(gs);
        }
    }

    public void RemoveSameGemstones()
    {
        for (int i = 0; i < sameGemstones.Count; i++)
        {
            Gemstone gs = sameGemstones[i] as Gemstone;
            RemoveOneGemstone(gs);
        }
        sameGemstones = new ArrayList();
        StartCoroutine(DetectSameKindAgain());
    }

    IEnumerator DetectSameKindAgain()
    {
        yield return new WaitForSeconds(0.5f);
        if (CheckKindHorizontally() || CheckKindVertically())
        {
            RemoveSameGemstones();
        }
    }

    public void RemoveOneGemstone(Gemstone gs)
    {
        gs.GemstoneDispose();
        AudioSource audio = GetComponent<AudioSource>();
        audio.PlayOneShot(sameKind);
        for (int i = gs.row + 1; i < row; i++)
        {
            Gemstone temp = GetGemstoneByPosition(i, gs.column);
            temp.row --;
            SetGemstonePosition(temp.row, temp.column, temp);
            //temp.GemStonePosition(temp.row, temp.column);
            temp.TweenPosition(temp.row, temp.column);
        }

        Gemstone newGemstone = AddGemstone(row, gs.column);
        newGemstone.row--;
        SetGemstonePosition(newGemstone.row, newGemstone.column, newGemstone);
        //newGemstone.GemStonePosition(newGemstone.row, newGemstone.column);
        newGemstone.TweenPosition(newGemstone.row, newGemstone.column);
    }

    public bool CheckKindHorizontally()
    {
        bool isSameKind = false;
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column - 2; j++)
            {
                if ((GetGemstoneByPosition(i, j).gemstoneKind == GetGemstoneByPosition(i, j + 1).gemstoneKind) && (GetGemstoneByPosition(i, j).gemstoneKind == GetGemstoneByPosition(i, j + 2).gemstoneKind))
                {
                    Debug.Log("same");
                    SaveSameGemstone(GetGemstoneByPosition(i, j));
                    SaveSameGemstone(GetGemstoneByPosition(i, j + 1));
                    SaveSameGemstone(GetGemstoneByPosition(i, j + 2));
                    isSameKind = true;
                }
            }
        }
        return isSameKind;
    }

    public bool CheckKindVertically()
    {
        bool isSameKind = false;
        for (int i = 0; i < column; i++)
        {
            for (int j = 0; j < row - 2; j++)
            {
                if ((GetGemstoneByPosition(j, i).gemstoneKind == GetGemstoneByPosition(j + 1, i).gemstoneKind) && (GetGemstoneByPosition(j, i).gemstoneKind == GetGemstoneByPosition(j + 2, i).gemstoneKind))
                {
                    Debug.Log("same");
                    SaveSameGemstone(GetGemstoneByPosition(j, i));
                    SaveSameGemstone(GetGemstoneByPosition(j + 1, i));
                    SaveSameGemstone(GetGemstoneByPosition(j + 2, i));
                    isSameKind = true;
                }
            }
        }
        return isSameKind;
    }
}
