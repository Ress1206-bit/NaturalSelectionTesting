using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] GameObject food;
    [SerializeField] float energy = 20f;
    [SerializeField] float waitTime = 1f;
    [SerializeField] GameObject femaleBlob;
    [SerializeField] GameObject maleBlob;


    //Genes
    [SerializeField] float speed;
    [SerializeField] float view;
    [SerializeField] float size;

    int x;
    int y;

    private Rigidbody rb;


    [SerializeField] int foodEnergy = 10;

    public LayerMask mask;

    private bool seeSomething = false;
    private bool isMovingRandomly = true;


    GameObject maleParentObject;
    GameObject femaleParentObject;

    private bool canJumpAway = false;

    void Start()
    {
        maleParentObject = GameObject.FindWithTag("maleHolder");
        femaleParentObject = GameObject.FindWithTag("femaleHolder");
        transform.localScale *= size;
        rb = GetComponent<Rigidbody>();
        StartCoroutine(MoveRandom());
    }

    void Update()
    {
        if(seeSomething)
        {
            rb.velocity = transform.forward * speed;
            isMovingRandomly = false;
        }

        if(energy <= 0)
        {
            Destroy(gameObject);
        }

        //Ray
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;

        if(Physics.Raycast(ray, out hitInfo, view, mask))
        {
            seeSomething = true;
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red);
        }
        else
        {
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * view, Color.green);
            seeSomething = false;
            if(!isMovingRandomly)
            {
                StartCoroutine(MoveRandom());
                isMovingRandomly = true;
            }

        } 
    }

    private IEnumerator MoveRandom()
    {
        while (energy > 0 && !seeSomething)
        {
            x = Random.Range(-1, 2);
            y = Random.Range(-1, 2);

            if(x == 1)
            {
                transform.rotation = Quaternion.Euler(0, 90, 0);
            } else if(x == -1)
            {
                transform.rotation = Quaternion.Euler(0, -90, 0);
            } else if(y == 1)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            } else if(y == -1)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }

            rb.velocity = new Vector3(x, 0, y) * speed;
            energy -= speed;

            yield return new WaitForSeconds(waitTime);
        }
    }

    private void OnCollisionEnter(Collision col) 
    {
        if(col.gameObject.CompareTag("food"))
        {
            Destroy(col.gameObject);
            energy += foodEnergy;
        }

        if(gameObject.tag == "female" && col.gameObject.CompareTag("male"))
        {
            StartCoroutine(Mating(col.gameObject));
        }

        if(gameObject.tag == "male" && col.gameObject.CompareTag(gameObject.tag))
        {
            if(size == col.gameObject.GetComponent<Movement>().getSize())
            {
                if(energy <= col.gameObject.GetComponent<Movement>().getEnergy())
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                if(size <= col.gameObject.GetComponent<Movement>().getSize())
                {
                    Destroy(gameObject);
                }
            }
        }

        if(gameObject.tag == "female" && col.gameObject.CompareTag(gameObject.tag))
        {
            if(energy > col.gameObject.GetComponent<Movement>().getEnergy())
            {
                transform.Translate(-1.5f, 0, -1.5f);
            }
            else
            {
                transform.Translate(1.5f, 0, 1.5f);
            }
        }
    }

    private void OnCollisionStay(Collision col)
    {
        if((canJumpAway && ((gameObject.tag == "female" && col.gameObject.tag == "male") || (gameObject.tag == "male" && col.gameObject.tag == "female"))) || (col.gameObject.CompareTag(gameObject.tag) && gameObject.tag == "female"))
        {
            Movement movement = col.gameObject.GetComponent<Movement>();
            if (movement != null && energy > movement.getEnergy())
            {
                transform.Translate(-1.5f, 0, -1.5f);
            }
            else
            {
                transform.Translate(1.5f, 0, 1.5f);
            }
        }
    }

    public float getEnergy()
    {
        return energy;
    }

    public float getSize()
    {
        return size;
    }

    //Only called on female Blobs
    private IEnumerator Mating(GameObject male)
    {
        // rb.constraints = RigidbodyConstraints.FreezeAll;
        // male.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        yield return new WaitForSeconds(2f);
        int boyOrGirl = Random.Range(0, 2); // 0 is girl and 1 is boy;
        if(boyOrGirl == 0)
        {
            GameObject babyGirlBlob = Instantiate(femaleBlob, new Vector3(2, 0, 2) + transform.position, Quaternion.identity);
            babyGirlBlob.transform.SetParent(femaleParentObject.transform);
            Movement momScript = GetComponent<Movement>();
            Movement dadScript = male.GetComponent<Movement>();
            Movement babyGirlScript = babyGirlBlob.GetComponent<Movement>();
            babyGirlScript.speed = momScript.speed + Random.Range(-1, 2) * 0.1f;
            babyGirlScript.view = momScript.view + Random.Range(-5, 6);
            babyGirlScript.size = momScript.size + Random.Range(-1, 2) * 0.25f;
            // rb.constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ);
            // male.GetComponent<Rigidbody>().constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ);
            transform.Rotate(0, 180, 0);
            male.transform.Rotate(0, 180, 0);
            transform.Translate(-1.5f, 0, -1.5f);
            male.transform.Translate(1.5f, 0, 1.5f);
            StartCoroutine(MoveRandom());
            StartCoroutine(dadScript.MoveRandom());
        }

        if(boyOrGirl == 1)
        {
            GameObject babyBoyBlob = Instantiate(maleBlob, new Vector3(2, 0, 2) + transform.position, Quaternion.identity);
            babyBoyBlob.transform.SetParent(maleParentObject.transform);
            Movement momScript = GetComponent<Movement>();
            Movement dadScript = male.GetComponent<Movement>();
            Movement babyBoyScript = babyBoyBlob.GetComponent<Movement>();
            babyBoyScript.speed = dadScript.speed + Random.Range(-1, 2) * 0.1f;
            babyBoyScript.view = dadScript.view + Random.Range(-5, 6);
            babyBoyScript.size = dadScript.size + Random.Range(-1, 2) * 0.25f;
            // rb.constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ);
            // male.GetComponent<Rigidbody>().constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ);
            transform.Rotate(0, 180, 0);
            male.transform.Rotate(0, 180, 0);
            transform.Translate(-1.5f, 0, -1.5f);
            male.transform.Translate(1.5f, 0, 1.5f);
            StartCoroutine(MoveRandom());
            StartCoroutine(dadScript.MoveRandom());
        }

        canJumpAway = true;

    }
}