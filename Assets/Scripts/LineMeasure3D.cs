using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Wave.Essence.Hand;

public class LineMeasure3D : MonoBehaviour
{
    public float pinchThreshold = 0.01f;

    public GameObject leftHandIndexTip;
    public GameObject leftHandThumbTip;
    GameObject leftHandMeasurePoint;

    public GameObject rightHandIndexTip;
    public GameObject rightHandThumbTip;
    GameObject rightHandMeasurePoint;

    public Light[] lightsToToggle;

    GameObject measureSpherePrefab;
    GameObject measureLinePrefab;
    GameObject measureTextPrefab;

    GameObject firstMeasurePoint;
    GameObject secondMeasurePoint;
    GameObject measureLine;
    GameObject measureText;

    private bool measuring;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(leftHandIndexTip != null);
        Debug.Assert(leftHandThumbTip != null);
        Debug.Assert(rightHandIndexTip != null);
        Debug.Assert(rightHandThumbTip != null);

        leftHandMeasurePoint = leftHandIndexTip;
        rightHandMeasurePoint = rightHandIndexTip;

        measuring = false;

        measureSpherePrefab = Resources.Load("Prefabs/MeasureSphere", typeof(GameObject)) as GameObject;
        measureLinePrefab = Resources.Load("Prefabs/MeasureLine", typeof(GameObject)) as GameObject;
        measureTextPrefab = Resources.Load("Prefabs/MeasureText", typeof(GameObject)) as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (HandManager.Instance != null)
        {
            var leftDist = (leftHandIndexTip.transform.position - leftHandThumbTip.transform.position).magnitude;
            var rightDist = (rightHandIndexTip.transform.position - rightHandThumbTip.transform.position).magnitude;

            if (leftDist <= pinchThreshold && rightDist <= pinchThreshold)
            {
                if (!measuring) // initiate measuring
                {
                    Debug.Log("Creating Left Hand Measure Point");
                    firstMeasurePoint = Instantiate(measureSpherePrefab);
                    firstMeasurePoint.name = "Left Hand Measure Point";
                    firstMeasurePoint.transform.SetParent(leftHandMeasurePoint.transform);
                    firstMeasurePoint.transform.localPosition = Vector3.zero;

                    Debug.Log("Creating Right Hand Measure Point");
                    secondMeasurePoint = Instantiate(measureSpherePrefab);
                    secondMeasurePoint.name = "Right Hand Measure Point";
                    secondMeasurePoint.transform.SetParent(rightHandMeasurePoint.transform);
                    secondMeasurePoint.transform.localPosition = Vector3.zero;

                    measureLine = Instantiate(measureLinePrefab);
                    measureLine.name = "Measure Line";

                    measureText = Instantiate(measureTextPrefab, Vector3.zero, Quaternion.identity);

                    foreach (var l in lightsToToggle)
                        l.enabled = false;                    

                    measuring = true;
                }
                else // actively measuring
                {
                    LineRenderer measureLineLineRenderer = measureLine.GetComponent<LineRenderer>();
                    measureLineLineRenderer.SetPosition(0, firstMeasurePoint.transform.position);
                    measureLineLineRenderer.SetPosition(1, secondMeasurePoint.transform.position);

                    //set measure line label
                    float distance = (secondMeasurePoint.transform.position - firstMeasurePoint.transform.position).magnitude;
                    Vector3 midpoint = (secondMeasurePoint.transform.position + firstMeasurePoint.transform.position) / 2;
                    measureText.transform.position = midpoint + new Vector3(0, 0.01f, 0);
                    TextMeshPro thisText = measureText.GetComponent<TextMeshPro>();
                    thisText.text = (distance * 1000).ToString("0.00") + "mm";
                }
            }
            else if (measuring == true)
            {
                Destroy(firstMeasurePoint);
                Destroy(secondMeasurePoint);
                Destroy(measureLine);
                Destroy(measureText);

                foreach (var l in lightsToToggle)
                    l.enabled = true;

                measuring = false;
            }
        }
    }
}
