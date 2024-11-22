using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class CustomCollider : MonoBehaviour
{
    public bool _enabled;
    public static bool IsCalling = false; // con esta controlo que no siempre que haya coalicion se haga swap
    public static VisualCard otherCard = null;
    void Start()
    {
        _enabled = true;
    }
    void OnTriggerEnter2D(Collider2D other)
    {

        if( _enabled && DragDrop.IsDragDrop)
        {
            otherCard = other.GetComponent<VisualCard>();
            VisualCard OnMyMouse = DragDrop.GetOnThisMoment();
           if(otherCard is not null && (otherCard.card.Type == CardType.clima || otherCard.card.Type == CardType.oro || otherCard.card.Type == CardType.plata))
           {
                 if (otherCard.card.Type == CardType.clima && OnMyMouse.card.Type == CardType.despeje)
                {
                    IsCalling = true;
                }
                else if((otherCard.card.Type == CardType.oro || otherCard.card.Type == CardType.plata) && OnMyMouse.card.Type == CardType.senuelo)
                {   
                    IsCalling = true;
                }
           }
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        IsCalling = false;
        otherCard = null;
    }
}
