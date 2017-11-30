Enable pins in Debian
=====================

Pins used by overlays:
https://github.com/jadonk/bonescript/blob/01eeb68f6b7b06076a990be24949377893c76907/dts/bs_template.dts

Edit `/boot/uEnv.txt` and uncomment:
* `disable_uboot_overlay_emmc`
* `disable_uboot_overlay_video`
* `disable_uboot_overlay_audio`

Notes
=====

* Do not add bonescript to `package.json`. Use the version that comes pre-installed.