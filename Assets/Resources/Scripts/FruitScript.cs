using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class FruitScript : MonoBehaviour
{
    public int type;
    public Material cross_section;
    public float force = 500;
    public GameObject smashed_version;
    public int value;

    public AudioClip[] slice_audios;
    public AudioClip smash_audio;
    public GameObject audio_instance;
    
    private GameObject game_controller;

    private Rigidbody rb;
    private Vector3 lastVelocity;
    private bool hasBeenHit = false;

    void changePoints(int point, bool hit) {
        game_controller.GetComponent<GameController>().updateScore(point, hit, type);
    }

    void playAudio(Vector3 position, bool is_slice) {
        GameObject creation = Instantiate(audio_instance, position, Quaternion.identity);
        if(is_slice) {
            creation.GetComponent<AudioSource>().clip = slice_audios[Random.Range(0, slice_audios.Length)];
        } else {
            creation.GetComponent<AudioSource>().clip = smash_audio;
        }
    }

    void setupSliced(GameObject component, Vector3 velocity, Collider[] weaponColliders) {
        component.tag = "SlicedFruit";
        component.name = "SlicedFruit";

        Rigidbody rigid = component.AddComponent<Rigidbody>();
        MeshCollider collider = component.AddComponent<MeshCollider>();
        collider.convex = true;
        rigid.linearVelocity = velocity;

        component.AddComponent<DecayScript>();

        // ignore collisions with ALL colliders on the weapon object
        Collider[] pieceColliders = component.GetComponentsInChildren<Collider>();
        foreach (var pieceCol in pieceColliders)
        {
            foreach (var weaponCol in weaponColliders)
            {
                Physics.IgnoreCollision(pieceCol, weaponCol);
            }
        }
    }

    void sliceFruit(Collider weapon) {
        // weapon is the child collider that hit us
        Transform plane = weapon.gameObject.transform.Find("SlicePlane").transform;

        SlicedHull hull = this.gameObject.Slice(plane.position, plane.up);

        // Vector3 velocity = this.gameObject.GetComponent<Rigidbody>().linearVelocity;
        Vector3 velocity = lastVelocity;

        // get ALL colliders on the weapon root object
        Collider[] weaponColliders = weapon.transform.root.GetComponentsInChildren<Collider>();

        if (hull != null)
        {
            GameObject upperHull = hull.CreateUpperHull(this.gameObject, cross_section);
            setupSliced(upperHull, velocity, weaponColliders);

            GameObject lowerHull = hull.CreateLowerHull(this.gameObject, cross_section);
            setupSliced(lowerHull, velocity, weaponColliders);
        }

        playAudio(this.gameObject.transform.position, true);

        Destroy(this.gameObject);
    }

    void setupSmashed(GameObject component, Vector3 velocity, Vector3 explosion_position) {
        component.tag = "SmashedFruit";
        Rigidbody rigid = component.GetComponent<Rigidbody>();
        rigid.linearVelocity = velocity;
        // rigid.AddExplosionForce(force, explosion_position, 10);
        component.GetComponent<Rigidbody>().linearVelocity = velocity;
    }

    void smashFruit() {
        GameObject creation = Instantiate(smashed_version, new Vector3(0, 0, 0), Quaternion.identity);
        creation.transform.position = this.gameObject.transform.position;
        creation.transform.rotation = this.gameObject.transform.rotation;

        // Vector3 velocity = this.gameObject.GetComponent<Rigidbody>().linearVelocity;
        Vector3 velocity = lastVelocity;

        foreach(Transform child in creation.transform) {
            setupSmashed(child.gameObject, velocity, creation.transform.position);
        }

        creation.transform.DetachChildren();
        playAudio(this.gameObject.transform.position, false);

        Destroy(creation);
        Destroy(this.gameObject);
    }

    void Start() {
        rb = GetComponent<Rigidbody>();
        game_controller = GameObject.FindGameObjectWithTag("GameController");
    }

    void FixedUpdate() {
        lastVelocity = rb.linearVelocity;
    }

    void OnTriggerEnter(Collider other) {
        // If already processed by smash or slice, ignore
        if (hasBeenHit)
            return;

        // however you detect slicing (tag, component, etc.)
        // example if you're using a "SlicingWeapon" tag on that collider:
        if (other.CompareTag("Slicing")) {
            hasBeenHit = true; // lock in
            changePoints(value, true);
            sliceFruit(other);
        }
    }


    void OnCollisionEnter(Collision collision) {
        // If already processed by slice, ignore
        if (hasBeenHit)
            return;

        GameObject hitGO = collision.collider.gameObject;

        // Example if you’re using tags for different hit types:
        if (hitGO.CompareTag("Smashing")) {
            hasBeenHit = true;
            changePoints(value, true);
            smashFruit();
            return;
        }

        switch (hitGO.tag) {
            case "Player":
                hasBeenHit = true;
                changePoints(-50, false);
                game_controller.GetComponent<GameController>().loseHeart();
                smashFruit();
                break;

            case "Hand":
                hasBeenHit = true;
                changePoints(value, true);
                smashFruit();
                break;

            case "Fruit":
            case "SlicedFruit":
            case "SmashedFruit":
            case "Block":
                // do nothing
                break;

            default:
                hasBeenHit = true;
                changePoints(-25, false);
                smashFruit();
                break;
        }
    }

}
