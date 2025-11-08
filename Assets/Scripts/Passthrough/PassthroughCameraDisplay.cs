using UnityEngine;
using PassthroughCameraSamples;

public class PassthroughCameraDisplay : MonoBehaviour
{
    public WebCamTextureManager webcamManager;
    public Renderer quadRenderer;
    private Texture2D texture;
    public string textureName;

    public float quadDistance = 0.5f;
    public float textureScale = 1.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (webcamManager.WebCamTexture != null)
        {
            PlaceQuad();
            quadRenderer.material.SetTexture(textureName, webcamManager.WebCamTexture);

        }

    }
    
    public void PlaceQuad()
    {
        Transform quadtransform = quadRenderer.transform;

        Pose cameraPose = PassthroughCameraUtils.GetCameraPoseInWorld(PassthroughCameraEye.Left);

        Vector2Int resolution = PassthroughCameraUtils.GetCameraIntrinsics(PassthroughCameraEye.Left).Resolution;

        quadtransform.position = cameraPose.position + cameraPose.forward * quadDistance;
        quadtransform.rotation = cameraPose.rotation;

        Ray leftSide = PassthroughCameraUtils.ScreenPointToRayInCamera(PassthroughCameraEye.Left, new Vector2Int(0, resolution.y / 2));
        Ray rightSide = PassthroughCameraUtils.ScreenPointToRayInCamera(PassthroughCameraEye.Left, new Vector2Int(resolution.x, resolution.y / 2));

        float horFov = Vector3.Angle(leftSide.direction, rightSide.direction);

        float quadScale = 2 * quadDistance * Mathf.Tan(horFov * Mathf.Deg2Rad / 2)*textureScale;

        float aspect = (float)webcamManager.WebCamTexture.height / (float)webcamManager.WebCamTexture.width;

        quadtransform.localScale = new Vector3(quadScale, quadScale * aspect, 1);
    }
}
