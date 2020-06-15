using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreatSwordScript : MonoBehaviour
{
    public Animator bossAnim;
    public GameObject dustExplosionPrefab;
    private float lastTime;
    public ParticleSystem[] swordEffects;
    public BoxCollider greatSwordCollider;
    public float checkSize;
    public Vector3 customSize;
    public Vector3 posOffset;
    public LayerMask hitLayers;
    private DamageDealer damageDealer;
    [HideInInspector]
    public bool betterColliders;

    private void Start()
    {
        damageDealer = this.GetComponent<DamageDealer>();
    }

    public struct BufferObj // estrutura que guarda informacoes das hitboxes
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 size;
        public Vector3 scale;
    }

    private LinkedList<BufferObj> trailList = new LinkedList<BufferObj>(); // lista encadeada
    private int maxFrameBuffer = 2;

    private void Update()
    {
        if (betterColliders)
        {
            CheckTrail();
        }
    }

    private void CheckTrail()
    {
        BufferObj bo = new BufferObj();
        bo.size = customSize;
        bo.scale = greatSwordCollider.transform.localScale;
        bo.rotation = greatSwordCollider.transform.rotation;
        bo.position = greatSwordCollider.transform.position + greatSwordCollider.transform.TransformDirection(greatSwordCollider.center + posOffset);
        trailList.AddFirst(bo);
        if (trailList.Count > maxFrameBuffer)
        {
            trailList.RemoveLast();
        }

        DetectTrailCollisions();
    }

    private void DetectTrailCollisions()
    {
        bool isFirstRound = true;
        BufferObj lastBo = new BufferObj();
        foreach (BufferObj bo in trailList) // para cada um dos azuis
        {
            if (!isFirstRound)
            {
                LinkedList<BufferObj> calculated = FillTrail(bo, lastBo);
                foreach (BufferObj cbo in calculated)
                {
                    Collider[] hits = Physics.OverlapBox(cbo.position, Vector3.Scale(bo.size, bo.scale), cbo.rotation, hitLayers, QueryTriggerInteraction.Ignore);

                    if (hits.Length > 0)
                    {
                        damageDealer.GreatSwordFiller(hits[0].gameObject);
                    }
                }
            }
            isFirstRound = false;
            lastBo = bo;
        }
    }

    private LinkedList<BufferObj> FillTrail(BufferObj from, BufferObj to) // preenche os gaps e retorna uma lista
    {
        LinkedList<BufferObj> fillerList = new LinkedList<BufferObj>();
        float distance = Mathf.Abs((from.position - to.position).magnitude);

        if (distance > checkSize)
        {
            float steps = Mathf.Ceil(distance / checkSize);
            float stepsAmount = 1 / (steps + 1);
            float stepValue = 0;
            for (int i = 0; i < (int)steps; i++)
            {
                stepValue += stepsAmount;
                BufferObj tmpBo = new BufferObj();
                tmpBo.size = customSize;
                tmpBo.position = Vector3.Lerp(from.position, to.position, stepValue);
                tmpBo.rotation = Quaternion.Lerp(from.rotation, to.rotation, stepValue);
                fillerList.AddFirst(tmpBo);
            }
        }
        return fillerList;
    }

    private void OnDrawGizmos()
    {
        bool isFirstRound = true;
        BufferObj lastBo = new BufferObj();
        foreach (BufferObj bo in trailList) // para cada um dos azuis
        {
            Gizmos.color = Color.blue;
            Gizmos.matrix = Matrix4x4.TRS(bo.position, bo.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.Scale(bo.size, bo.scale)); // desenha o collider
            if (!isFirstRound)
            {
                LinkedList<BufferObj> calculated = FillTrail(bo, lastBo);
                foreach (BufferObj cbo in calculated)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.matrix = Matrix4x4.TRS(cbo.position, cbo.rotation, Vector3.one);
                    Gizmos.DrawWireCube(Vector3.zero, Vector3.Scale(bo.size, bo.scale));

                    Collider[] hits = Physics.OverlapBox(cbo.position, cbo.size, cbo.rotation, hitLayers, QueryTriggerInteraction.Ignore);
                }
            }
            isFirstRound = false;
            lastBo = bo;
        }
    }

    public void EnableGreatSwordFire()
    {
        this.transform.GetChild(1).gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((bossAnim.GetBool("Attacking")/* || other.gameObject.layer == 9)*/ && /*(other.gameObject.layer == 9 || */other.gameObject.layer == 13) && Time.time > lastTime + 0.1f) // Ground ou Scenary
        {
            if(dustExplosionPrefab != null)
            {
                GameObject dustEx = Instantiate(dustExplosionPrefab, this.transform.position, Quaternion.identity);
                Destroy(dustEx, 1);
            }
            lastTime = Time.time;
        }
    }

}
