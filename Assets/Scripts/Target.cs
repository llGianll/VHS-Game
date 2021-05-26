using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour, IShootable
{
    [SerializeField] float _maxHP;
    [SerializeField] Color _hitColor;

    float _currentHP;
    Color _originalColor;
    MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        _currentHP = _maxHP;
        _originalColor = _meshRenderer.material.color;
    }

    private void Update()
    {
        _meshRenderer.material.color = _originalColor;
    }

    public void Hit()
    {
        _meshRenderer.material.color = _hitColor;
    }
}
