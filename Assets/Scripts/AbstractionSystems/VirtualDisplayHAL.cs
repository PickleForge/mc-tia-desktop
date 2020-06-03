using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class VirtualDisplayHAL : MonoBehaviour
{
    // Start is called before the first frame update
    private Texture2D raster;
    private static bool test_running = false;

    private int width = 50;
    private int height = 50;

    public int getWidth() {
        if (this.raster == null) return 0;
        return this.raster.width;
    }
    public int getHeight() {
        if (this.raster == null) return 0;
        return this.raster.height;
    }

    void Start()
    {
        // Define attributes
        this.raster = new Texture2D(this.width, this.height);

        // Run test!
        // run_debug_tests();
    }

    void flush() {
        RawImage view = gameObject.GetComponent<RawImage>();
        this.raster.Apply();
        view.texture = this.raster;
    }

    // Update is called once per frame
    private int blanks = 0;
    void Update()
    {
        blanks++;
        if (blanks == 10) {
            // print("blank_flush");
            blanks = 0;
            flush();
        }
        // print("RawImage Update Done");
    }

    private Color makeColor(int r, int g, int b)
    {
        return new Color(r / 255.0F, g / 255.0F, b / 255.0F);
    }

    public void set_pixel(int x, int y, int r, int g, int b)
    {
        Color targetColor = this.makeColor(r, g, b);
        this.raster.SetPixel(x, (this.raster.height - y), targetColor);
        // this.raster.Apply();
        // (GetComponent<RawImage>().texture as Texture2D).SetPixel(x, y, targetColor);
    }

    void run_debug_tests()
    {
        StartCoroutine(run_debug_test_1());
        // Invoke("run_debug_test_2", 2);
    }

    IEnumerator run_debug_test_1()
    {
        if (test_running) { yield break; }
        test_running = true;
        print("Running debug test!");
        int parity = 0;
        for (int y = 0; y < this.raster.height; y++) {
            for (int x = 0; x < this.raster.width; x++) {
                if (parity == 0) {
                    set_pixel(x, y, 255, 0, 0);
                    parity = 1;
                    print("(" + x + "," + y + ") R");
                } else if (parity == 1) {
                    set_pixel(x, y, 0, 255, 0);
                    parity = 2;
                    print("(" + x + "," + y + ") G");
                } else if (parity == 2) {
                    set_pixel(x, y, 0, 0, 255);
                    parity = 0;
                    print("(" + x + "," + y + ") B");
                } else {
                    print("(" + x + "," + y + ") Umm");
                }
            }
            yield return new WaitForSeconds(.1f);
        }
        print("Finished debug test!");
        test_running = false;
        yield break;
    }

    void run_debug_test_2()
    {
        for (int y = 0; y < this.height; y++) {
            for (int x=0; x < this.width; x++) {
                if (y % 2 == 0) {
                    set_pixel(x, y, 255, 0, 0);
                } else {
                    set_pixel(x, y, 0, 0, 255);
                }
            }
        }
    }
}
