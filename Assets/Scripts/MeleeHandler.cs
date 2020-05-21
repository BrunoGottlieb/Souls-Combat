using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeleeHandler : MonoBehaviour
{
    public WeaponHandler weaponHandlerRef;

    public LayerMask hitLayers;

    public bool debugTrail = false;

    public struct BufferObj
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 size;
    }

    private LinkedList<BufferObj> trailList = new LinkedList<BufferObj>();
    LinkedList<BufferObj> trailFillerList = new LinkedList<BufferObj>();
    private int maxFrameBuffer = 2;
    private BoxCollider weaponCollider;
    Animator anim;

    int attackId = 0;

    void Start()
    {
        anim = this.GetComponentInChildren<Animator>();
        weaponCollider = (BoxCollider)weaponHandlerRef.weapon.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // clique esquerdo = heavy
        {
            SetAttack(0);
        } else if (Input.GetMouseButtonDown(1)) // clique direito = light
        {
            SetAttack(1);
        }
        if (anim.GetBool("IsDamageOn"))
        {
            CheckTrail();
        }
    }

    private void SetAttack(int attackType)
    {
        if (anim.GetBool("CanAttack"))
        {
            attackId++;
            anim.SetTrigger("Attack");
            anim.SetInteger("AttackType", attackType);
        }
    }

    private void CheckTrail()
    {
        BufferObj bo = new BufferObj();
        bo.size = weaponCollider.size;
        bo.rotation = weaponCollider.transform.rotation;
        bo.position = weaponCollider.transform.position + weaponCollider.transform.TransformDirection(weaponCollider.center);
        trailList.AddFirst(bo);
        if (trailList.Count > maxFrameBuffer)
        {
            trailList.RemoveLast();
        }
        else if (trailList.Count > 1)
        {
            trailFillerList = FillTrail(trailList.First.Value, trailList.Last.Value);
        }

        Collider[] hits = Physics.OverlapBox(bo.position, bo.size / 2, bo.rotation, hitLayers, QueryTriggerInteraction.Ignore);

        Dictionary<long, Collider> colliderList = new Dictionary<long, Collider>();
        CollectColliders(hits, colliderList);
        foreach (BufferObj cbo in trailFillerList)
        {
            hits = Physics.OverlapBox(cbo.position, cbo.size / 2, cbo.rotation, hitLayers, QueryTriggerInteraction.Ignore);
            CollectColliders(hits, colliderList);
        }
        foreach(Collider collider in colliderList.Values)
        {
            HitData hd = new HitData();
            hd.id = attackId;
            Hittable hittable = collider.GetComponent<Hittable>();
            if (hittable)
            {
                hittable.Hit(hd);
                print("Hittable");
            }
            print("Hit");
        }
    }

    private static void CollectColliders(Collider[] hits, Dictionary<long, Collider> colliderList)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            if (colliderList.ContainsKey(hits[i].GetInstanceID()))
            {
                colliderList.Add(hits[i].GetInstanceID(), hits[i]);
            }
        }
    }

    private LinkedList<BufferObj> FillTrail(BufferObj from, BufferObj to)
    {
        LinkedList<BufferObj> fillerList = new LinkedList<BufferObj>();
        float distance = Mathf.Abs((from.position - to.position).magnitude);
        if(distance > weaponCollider.size.z)
        {
            float steps = Mathf.Ceil(distance / weaponCollider.size.z);
            float stepsAmount = 1 / (steps + 1);
            float stepValue = 0;
            for (int i = 0; i < (int)steps; i++)
            {
                stepValue += stepsAmount;
                BufferObj tmpBo = new BufferObj();
                tmpBo.size = weaponCollider.size;
                tmpBo.position = Vector3.Lerp(from.position, to.position, stepValue);
                tmpBo.rotation = Quaternion.Lerp(from.rotation, to.rotation, stepValue);
                fillerList.AddFirst(tmpBo);
            }
        }
        return fillerList;
    }

    private void OnDrawGizmos()
    {
        if (debugTrail)
        {
            foreach (BufferObj bo in trailList)
            {
                Gizmos.color = Color.blue;
                Gizmos.matrix = Matrix4x4.TRS(bo.position, bo.rotation, Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, bo.size);
            }
            foreach (BufferObj bo in trailFillerList)
            {
                Gizmos.color = Color.yellow;
                Gizmos.matrix = Matrix4x4.TRS(bo.position, bo.rotation, Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, bo.size);
            }
        }
    }

}
