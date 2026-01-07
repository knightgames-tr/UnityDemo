using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaintManager : MonoBehaviour
{
    public static PaintManager Instance;
    void Awake(){
        Instance = this;
    }

    GameManager _gameManager;
    PlayerController _playerController;
    void Start(){
        _gameManager = GameManager.Instance;
        _playerController = PlayerController.Instance;

        setPaintTexture();
    }

    void Update()
    {
        if(_isPaintActive){
            paintTexture();
        }
    }

    bool _isPaintActive;
    [ContextMenu ("activatePaint")]
    public void activatePaintManager(){
        _board.DOScale(Vector3.one,1f);
        _gameManager.deactivateMoneyUI();
        rotatePlayerToBoard();
        _isPaintActive = true;
        _paintUI.SetActive(true);
        _brushSizeSlider.onValueChanged.AddListener((v) => {setBrushSize(v);});
    }

    void rotatePlayerToBoard(){
        _playerController._playerModel.transform.DOLookAt(_paintArea.transform.position, 0.4f, AxisConstraint.Y);
    }

    public Transform _board;
    public Renderer _paintArea;
    int _paintTexturePixelCount;
    int _paintedPixelCount;
    Texture2D _paintTexture;
    MaterialPropertyBlock _materialPropertyBlock;
    int _paintTextureSize = 50;
    int _paintTextureWidthRatio = 16;
    int _paintTextureHeightRatio = 9;
    Color[] _pixelColors;
    void setPaintTexture(){
        _paintTexture = new Texture2D(_paintTextureWidthRatio * _paintTextureSize,_paintTextureHeightRatio * _paintTextureSize, TextureFormat.RGBA32, false);
        _paintTexture.filterMode = FilterMode.Point;
        _paintTexturePixelCount = _paintTexture.width * _paintTexture.height;

        //Set all pixels to white
        _pixelColors= new Color[_paintTexture.width * _paintTexture.height];
        for (int i = 0; i < _pixelColors.Length; i++)
        {
            _pixelColors[i] = Color.white;
        }

        applyPixelColorsToTexture();
    }

    void applyPixelColorsToTexture(){
        //Apply texture to renderer
        _paintTexture.SetPixels(_pixelColors);
        _paintTexture.Apply();

        _materialPropertyBlock = new MaterialPropertyBlock();
        _materialPropertyBlock.SetTexture("_BaseMap", _paintTexture);
        _paintArea.SetPropertyBlock(_materialPropertyBlock);

        updatePaintedPercentageText();
    }

    public LayerMask _paintLayer;
    void paintTexture(){        
        if (Input.GetMouseButton(0)){
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 50, _paintLayer)){

                //Get hit point pixel position on uv
                Vector2 textureCoord = hit.textureCoord;
                int pixelX = (int) (textureCoord.x * _paintTexture.width);
                int pixelY = (int) (textureCoord.y * _paintTexture.height);
                Vector2Int paintPixelPosition = new Vector2Int(pixelX,pixelY);

                drawCircleOnTexture(paintPixelPosition);
            }
        }
    }

    void drawCircleOnTexture(Vector2Int centerCoordinate){
        //Find what pixels to draw on, using brush size radius
        int brushSizeSquare = _brushSize*_brushSize;

        for (int y = -_brushSize; y <= _brushSize; y++){
            for (int x = -_brushSize; x <= _brushSize; x++){
                if (x * x + y * y <= brushSizeSquare){
                    int px = centerCoordinate.x + x;
                    int py = centerCoordinate.y + y;

                    // Check if outside of texture
                    if (px < 0 || px >= _paintTexture.width || py < 0 || py >= _paintTexture.height){
                        continue;
                    }

                    int index = py * _paintTexture.width + px;
                    if(_pixelColors[index] == Color.white){
                        _paintedPixelCount++;
                    }
                    _pixelColors[index] = _brushColor;
                }
            }
        }

        applyPixelColorsToTexture();
    }

    [Header ("UI Elements")]
    public GameObject _paintUI;
    public Slider _brushSizeSlider;
    int _brushSize = 10;
    int _brushSizeMultiplier = 10;
    public void setBrushSize(float value){
        _brushSize = (int)value*_brushSizeMultiplier;
    }

    public List<Transform> _colorButtons;
    Color _brushColor = Color.yellow;
    int _currentButtonIndex;
    public void setBrushColor(int buttonNo){
        _colorButtons[_currentButtonIndex].DOScale(Vector3.one,1f);
        _colorButtons[buttonNo].DOScale(Vector3.one*1.15f,0.4f);
        _currentButtonIndex = buttonNo;

        if(buttonNo == 0){
            _brushColor = Color.yellow;
        }else if(buttonNo == 1){
            _brushColor = Color.red;
        }else if(buttonNo == 2){
            _brushColor = Color.cyan;
        }
    }

    public TextMeshProUGUI _paintedPercentageText;
    public void updatePaintedPercentageText(){
        _paintedPercentageText.text = ( (int) (((float)_paintedPixelCount/_paintTexturePixelCount)*100) )+"%";

    }

}
