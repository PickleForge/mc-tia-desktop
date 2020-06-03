using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLauncher : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        // Fetch the display
        print("Getting graphics view!");
        GameObject uiview = GameObject.FindWithTag("VirtualDisplay");
        VirtualDisplayHAL display = uiview.GetComponent<VirtualDisplayHAL>();

        // Create and launch a system!
        print("Building system components");
        CPUCore mainCore = gameObject.AddComponent<CPUCore>();
        ALU alu0 = gameObject.AddComponent<ALU>();
        Memory ram = gameObject.AddComponent<Memory>();
        CPU processor = gameObject.AddComponent<CPU>();

        print("Building boot disk!");
        Memory bootDisk = getBootDisk();

        print("Building RAM!");
        ram.Instantiate(1024);

        print("Building CORE0");
        alu0.Instantiate(mainCore);
        mainCore.setALU(alu0);

        print("Building CPU");
        processor.Instantiate(new CPUCore[] { mainCore }, ram, bootDisk, display);
        mainCore.setParent(processor);

        print("Booting!");
        processor.bootup();
    }

    private Memory getBootDisk() {
        Memory memory = gameObject.AddComponent<Memory>();

        memory.Instantiate(new uint[] {
            0x03, 0x07, 0x03,
            0x01, 0x10,
            0x04, 0x0A, 0x08,
            0x04, 0x0E, 0x09,
            0x03, 0x08, 0x00,
            0x03, 0x09, 0x00,
            0x08, 0x05, 0x00,
            0x0A, 0x1B,
            0x03, 0x0B, 0xFF,
            0x3A, 0x1E,
            0x03, 0x0B, 0x00,
            0x04, 0x0C, 0x0B,
            0x04, 0x0D, 0x0B,
            0x03, 0x07, 0x01,
            0x01, 0x10,
            0x24, 0x05,
            0x12, 0x08, 0x01,
            0x09, 0x08, 0x0A,
            0x2E, 0x35,
            0x3A, 0x11,
            0x03, 0x08, 0x00,
            0x12, 0x09, 0x01,
            0x24, 0x05,
            0x09, 0x09, 0x0E,
            0x2E, 0x44,
            0x3A, 0x11,
            0x25
        });

        return memory;
    }

    // Update is called once per frame
    void Update() {
        
    }
}
