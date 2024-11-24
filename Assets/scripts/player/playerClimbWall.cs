using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEditor.UI;
using UnityEngine;

public class playerClimbWall : MonoBehaviour, IplayerFeature
{
    private characterController characterController;
    private ContactFilter2D contactFilter = new ContactFilter2D();
    private string defaultLayerName = "blueSlimeArea";
    public LayerMask layerMask;

    public blueSlime currentBlueSlime;

    private float currentPosOnLine;

    public void Awake()
    {
        layerMask = LayerMask.GetMask(defaultLayerName);

        contactFilter.useTriggers = true;
        contactFilter.useLayerMask = true;
        contactFilter.layerMask = layerMask;
    }

    public void FixedUpdate()
    {
        if(currentBlueSlime == null)
        {
            return;
        }

        currentPosOnLine += -characterController.returnMoveInput().y / 100;
        currentPosOnLine = Mathf.Clamp(currentPosOnLine, 0, 1);

        getPositionOnLine(currentBlueSlime.getLine(), currentPosOnLine, out Vector2 pos);
        characterController.rb.MovePosition(pos);
    }

    public void initFeauture(characterController characterController)
    {
        this.characterController = characterController;
    }

    public void triggerFeauture(bool useInput = false, bool input = false)
    {
        List<Collider2D> colliders = new List<Collider2D>();

        characterController.rb.OverlapCollider(contactFilter, colliders);

        if(colliders.Count > 0)
        {
            if(currentBlueSlime == null)
            {
                currentBlueSlime = colliders[0].GetComponent<blueSlime>();

                characterController.disableMovement();

                getClosestPointOnLine(out Vector2 closestPointOnLine, out float posOnLine);
                characterController.rb.MovePosition(closestPointOnLine);
                characterController.rb.velocity = Vector2.zero;

                currentPosOnLine = posOnLine;
            }else
            {
                endFeauture();
            }
        }
    }

    public void endFeauture()
    {
        currentBlueSlime = null;

        characterController.enableMovement();
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        if(LayerMask.LayerToName(collider.gameObject.layer) == defaultLayerName)
        {
            endFeauture();
        }
    }

    public bool getClosestPointOnLine(out Vector2 closestPointOnLine, out float posOnLine)
    {
        if(currentBlueSlime != null)
        {
            blueSlime.line line = currentBlueSlime.getLine();

            Vector2 lineVector = line.pointB - line.pointA;
            Vector2 playerVector = characterController.rb.position - line.pointA;
            
            float projectionLength = Vector3.Dot(playerVector, lineVector) / Vector3.Dot(lineVector, lineVector);
            float t = Mathf.Clamp01(projectionLength);
        
            closestPointOnLine = line.pointA + t * lineVector; 

            posOnLine = t;
            return true;
        }

        posOnLine = -1;
        closestPointOnLine = Vector2.zero;
        return false;
    }

    public bool getPositionOnLine(blueSlime.line line, float t, out Vector2 position)
    {
        position = line.pointA + t * (line.pointB - line.pointA);
        return true;
    }
}
