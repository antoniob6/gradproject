using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PickUpStuff : MonoBehaviour
{


    public float force = 5;


    private PlayerConnectionObject parentCO;
    private bool active;

    private GameObject selecteObject;
    private void Start() {
        parentCO = GetComponentInParent<PlayerConnectionObject>();
        active = parentCO.active;
        if (active) {
           // print("pick up stuff script active");
        }
    }


    private void Update() {
        if (active && Input.GetMouseButtonDown(0) ) {
            RaycastHit hit;
            Ray ray = parentCO.getPlayerCamera().ScreenPointToRay(Input.mousePosition);
            //Debug.Log("trying to identify");
            RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray);

            if (hit2D.collider != null) {
              //  Debug.Log("clicked on something 2d:"+ hit2D.collider.name);
                selecteObject = hit2D.collider.gameObject;
                return;
            }

            if (Physics.Raycast(ray, out hit, 100.0f)) {
               
                if (hit.transform != null) {
                  //  Debug.Log("clicked on something 3d:"+ hit.transform.gameObject.name);
                    selecteObject = hit.transform.gameObject;

                }
            }
        }
        else if (active && Input.GetMouseButtonDown(0)) {
            Debug.Log("trying to identify");
            Collider2D clickedObj= Physics2D.OverlapCircle(Input.mousePosition, 5f);
            if (clickedObj) {
                selecteObject = clickedObj.gameObject;
                Debug.Log("clicked on something:" + clickedObj.name);

            }

            
        } else if(active && Input.GetMouseButtonDown(1)) {
            
            Vector3 mousePosition = 
                    parentCO.getPlayerCamera().ScreenToWorldPoint(Input.mousePosition);

            selecteObject.transform.position = 
                new Vector3(mousePosition.x, mousePosition.y, 0);
        }
    }










}
