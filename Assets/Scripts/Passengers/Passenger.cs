using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Passenger : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] private AudioSource audioSourceRun;
    [SerializeField] private AudioClip audioClipJump;

    private MyColor myColor;
    private Material material;

    public Vector3 Position => transform.position;

    public void Init()
    {
        animator ??= GetComponentInChildren<Animator>();
        meshRenderer ??= GetComponentInChildren<SkinnedMeshRenderer>();
        SetMoving(false);
    }

    public void SetMaterial(Material mat)
    {
        material = mat;
        meshRenderer.material = mat;
    }

    public Material GetMaterial() => material;

    public bool IsMaterialEqual(Material other)
    {
        return material == other;
    }

    public void SetColor(MyColor color)
    {
        myColor = color;
    }

    public MyColor GetColor() => myColor;

    public void SetMoving(bool value)
    {
        if(value == true)
        {
            audioSourceRun.Play();
        }
        else
        {
            audioSourceRun.Stop();
        }
        if (animator != null)
            animator.SetBool("IsMoving", value);
    }

    public Tweener MoveTo(Vector3 target, float duration)
    {
        return transform.DOMove(target, duration)
            .SetEase(Ease.Linear)
            .OnStart(() => {  SetMoving(true); })
            .OnComplete(() => { SetMoving(false); });
    }

    public Sequence LocalJump(Vector3 target, float power, int numJumps, float duration)
    {
        return transform.DOLocalJump(target, power, numJumps, duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => AudioSource.PlayClipAtPoint(audioClipJump,Vector3.zero));
    }

    public void SetPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    public void LookAtDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.001f) return;
        transform.rotation = Quaternion.LookRotation(direction.normalized);
    }

    public Tweener MoveAlongPath(List<Vector3> wayPoints, float duration)
    {
        return transform.DOPath(wayPoints.ToArray(), duration, PathType.Linear, PathMode.Full3D).SetLookAt(0.01f)
            .SetEase(Ease.Linear);
    }
}
