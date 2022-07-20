using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MFD_Map : MFD_Page
{
    [SerializeField] private TextMeshProUGUI selectedMissionText;
    [SerializeField] private RectTransform mapTransform;
    [SerializeField] private GameObject objectiveMarkerTemplate;
    [SerializeField] private GameObject playerMarkerTemplate;

    private List<GameObject> markers = new List<GameObject>();

    public override void Display()
    {
        foreach (GameObject m in markers)
            Destroy(m);

        markers.Clear();

        GameObject player = GameController.Instance.Player;
        if(player != null)
            CreateMarker(playerMarkerTemplate, player.transform.position);

        Mission msn = MissionController.Instance.GetMission(IngameMenu.Instance.SelectedMission);
        if (msn != null)
        {
            selectedMissionText.text = "MISSION " + (IngameMenu.Instance.SelectedMission + 1).ToString();                       
            var positions = msn.GetMarkerPositions();
            foreach(Vector3 pos in positions)
            {
                CreateMarker(objectiveMarkerTemplate, pos);
            }
        }
    }

    public override void ProcessButton(int idx)
    {
        switch (idx)
        {
            case 7:
                IngameMenu.Instance.SelectedMission--;
                Display();
                break;
            case 8:
                IngameMenu.Instance.SelectedMission++;
                Display();
                break;
        }
    }

    private Vector3 GetMapCoords(Vector3 mapPos)
    {
        Vector3[] corners = new Vector3[4];
        mapTransform.GetWorldCorners(corners);
        Vector3 mapSize = corners[2] - corners[0];

        return corners[0] + new Vector3(mapPos.x * mapSize.x, mapPos.y * mapSize.y, 0.0f);
    }

    private void CreateMarker(GameObject template, Vector3 pos)
    {
        GameObject obj = Instantiate(template, transform);
        RectTransform tr = obj.GetComponent<RectTransform>();
        tr.position = GetMapCoords(GameController.Instance.WorldToMapPos(pos));
        markers.Add(obj);
        obj.SetActive(true);
    }
}
