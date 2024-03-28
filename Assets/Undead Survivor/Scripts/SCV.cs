using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class SCV : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float moveSpeed = 5f;
    private float miningTime = 3f;
    private bool isTaking = false;
    private bool isMining = false;

    public GameObject mineralPrefab;

    private GameObject targetMineral;
    
    private Vector3 mineralPosition;
    
    private float miningTimer; 
    Rigidbody2D rigid;

    GameObject center;
    void Start()

    {
        rigid = GetComponent<Rigidbody2D>();
        center = GameObject.FindGameObjectWithTag("Center");
        //가까운 미네랄 찾음.
        FindMinerals();
    }

    // Update is called once per frame
    void Update()
    {
        
        //미네랄 향해서 움직임.
        MoveToMinerals();
    
        //미네랄 캘 때 멈춤 (3초)
        //미네랄 다 캐면 미네랄 듦
        //미네랄 덩어리 인자로 받아옴
        if(isMining){
            miningTimer += Time.deltaTime;
            if(miningTimer >= miningTime){
                GetMinerals();
                miningTimer = 0;
                isMining = false;
                isTaking = true;
            }
        }
        if(isTaking){
            MoveToCenter();
        }

        DepositMinerals();
        //센터에 닿으면 미네랄 소멸
        //미네랄 ++
    }
    void FindMinerals(){
        GameObject[] minerals = GameObject.FindGameObjectsWithTag("Minerals");
        float closestDistance = Mathf.Infinity;
        foreach (GameObject mineral in minerals)
        {
            float distance = Vector3.Distance(transform.position, mineral.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                targetMineral = mineral;
                mineralPosition = mineral.transform.position;
            }
        }
    }   

    void GetMinerals(){
        Transform mineralPos = GetComponentInChildren<Transform>(); 
        GameObject money = Instantiate(mineralPrefab, mineralPos.position , Quaternion.identity);
    }
    void MoveToMinerals(){
        if(!isMining)
            rigid.MovePosition(transform.position + (mineralPosition - transform.position).normalized * moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, mineralPosition) < 0.1f)
        {
            isMining = true;
        }
    }

    void MoveToCenter(){
        if(isTaking)
            rigid.MovePosition(transform.position + (center.transform.position - transform.position).normalized * moveSpeed * Time.deltaTime);
    }
    void DepositMinerals(){

        //가까운 미네랄 찾음.
        FindMinerals();
    }
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Minerals"){
            isMining = true;
        }
        
        else if(other.gameObject.tag == "Center"){
            DepositMinerals();
        }   
    }
}
