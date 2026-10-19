using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CargoDamageSystemTests
{
    private GameObject _go;
    private CargoDamageSystem _damage;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("TestTruck");
        _damage = _go.AddComponent<CargoDamageSystem>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_go);
    }

    [Test]
    public void CalculateDamage_HighSpeed_Returns2Damage()
    {
        // Act
        int result = _damage.calculateDamage(8.0f);
        // Assert
        Assert.AreEqual(2, result);
    }

    [Test]
    public void CalculateDamage_ExactHighThreshold_Returns2Damage()
    {
        int result = _damage.calculateDamage(7.5f);
        Assert.AreEqual(2, result);
    }

    [Test]
    public void CalculateDamage_MediumSpeed_Returns1Damage()
    {
        int result = _damage.calculateDamage(6.0f);
        Assert.AreEqual(1, result);
    }

    [Test]
    public void CalculateDamage_LowSpeed_Returns0Damage()
    {
        int result = _damage.calculateDamage(3.0f);
        Assert.AreEqual(0, result);
    }
    
}
