using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordScript : MonoBehaviour
{
    public Animator girlAnim; // animator do player
    private AudioSource sparksSource; // audioSource que ira tocar o som, aleatoriamente
    public GameObject sparkEffect; // prefab das faiscas
    public AudioClip[] sparkSound; // som quando a espada bate em algo
    public bool debug;
    private BoxCollider swordCollider;

    private void Start()
    {
        sparksSource = this.GetComponent<AudioSource>();
        swordCollider = this.GetComponent<BoxCollider>();
    }

    public struct BufferObj // estrutura que guarda informacoes das hitboxes
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 size;
        public Vector3 scale;
    }
    private LinkedList<BufferObj> trailList = new LinkedList<BufferObj>(); // lista encadeada
    private LinkedList<BufferObj> trailFillerList = new LinkedList<BufferObj>();
    private int maxFrameBuffer = 10;

    private void Update()
    {
        if (debug)
        {
            CheckTrail();
        }
    }

    private void CheckTrail()
    {
        BufferObj bo = new BufferObj();
        bo.size = swordCollider.size;
        bo.scale = swordCollider.transform.localScale;
        bo.rotation = swordCollider.transform.rotation;
        bo.position = swordCollider.transform.position + swordCollider.transform.TransformDirection(swordCollider.center) / 2;
        trailList.AddFirst(bo);
        if (trailList.Count > maxFrameBuffer)
        {
            trailList.RemoveLast();
        }
        //if (trailList.Count > 1)
        //{
        //    print("0");
            trailFillerList = FillTrail(trailList.First.Value, trailList.Last.Value);
       // }
    }

    private LinkedList<BufferObj> FillTrail(BufferObj from, BufferObj to)
    {
        LinkedList<BufferObj> fillerList = new LinkedList<BufferObj>();
        float distance = Mathf.Abs((from.position - to.position).magnitude);
        if (distance > swordCollider.size.z/4)
        {
            float steps = Mathf.Ceil(distance / swordCollider.size.z);
            float stepsAmount = 1 / (steps + 1);
            float stepValue = 0;
            for (int i = 0; i < (int)steps; i++)
            {
                stepValue += stepsAmount;
                BufferObj tmpBo = new BufferObj();
                tmpBo.size = swordCollider.size;
                tmpBo.position = Vector3.Lerp(from.position, to.position, stepValue);
                tmpBo.rotation = Quaternion.Lerp(from.rotation, to.rotation, stepValue);
                fillerList.AddFirst(tmpBo);
            }
        }
        return fillerList;
    }

    private void OnDrawGizmos()
    {
        foreach (BufferObj bo in trailList)
        {
            Gizmos.color = Color.blue;
            Gizmos.matrix = Matrix4x4.TRS(bo.position, bo.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.Scale(bo.size, bo.scale));
        }
        foreach (BufferObj bo in trailFillerList)
        {
            print("a");
            Gizmos.color = Color.yellow;
            Gizmos.matrix = Matrix4x4.TRS(bo.position, bo.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.Scale(bo.size, bo.scale));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 13 && girlAnim.GetBool("Attacking") && !sparksSource.isPlaying) // caso seja um elemento do cenario
        {
            sparksSource.clip = sparkSound[UnityEngine.Random.Range(0, sparkSound.Length)]; // toca um som de sword hit aleatorio
            sparksSource.Play(); // toca um som de sword hit aleatorio
            if (other.transform.parent.gameObject.name.Contains("Rochas") || other.transform.parent.gameObject.name.Contains("Arco")) // caso seja uma rocha, instancia o efeito
            {
                GameObject spark = Instantiate(sparkEffect, this.transform.position, Quaternion.identity); // instancia as faiscas
                Destroy(spark, 0.3f);
            }
        }
    }

}
