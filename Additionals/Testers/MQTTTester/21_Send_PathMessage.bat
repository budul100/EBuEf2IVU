@ECHO OFF

SET MQTTBROKER=localhost
SET MQTTTOPIC=mqtt4455
SET MQTTMESSAGE=[ { "abfahrt": "15:03", "abfahrt_exakt": "15:03:00", "abfahrt_ist": null, "abfahrt_plan": "15:03:00", "abfahrt_prognose": null, "abfahrt_soll": "15:02:00", "ankunft": null, "ankunft_exakt": null, "ankunft_ist": null, "ankunft_plan": null, "ankunft_prognose": null, "ankunft_soll": null, "bemerkungen": "", "betriebsstelle": "XAB", "bremssystem": "", "fahrtrichtung": "11", "fzm_id": "380679", "gleis": "1", "gleis_ist": null, "gleis_plan": "1", "gleis_soll": "1", "ins_gegengleis": "0", "ist_durchfahrt": "0", "ist_kurzeinfahrt": "0", "mbr": "0", "triebfahrzeug": "646", "triebfahrzeug_ist": "646", "uebergang_nach_zug_id": "31050", "uebergang_von_zug_id": null, "verkehrstage": "Fr", "verkehrstage_bin": "0000100", "vmax": "120", "vmax_ist": "120", "wendezug": "0", "zug": "IRE 5029", "zug_id": "31046", "zuggattung": "IRE", "zuggattung_id": "15", "zugnummer": "5029" }, { "abfahrt": null, "abfahrt_exakt": null, "abfahrt_ist": null, "abfahrt_plan": null, "abfahrt_prognose": null, "abfahrt_soll": null, "ankunft": "15:28", "ankunft_exakt": "15:28:00", "ankunft_ist": null, "ankunft_plan": "15:28:00", "ankunft_prognose": null, "ankunft_soll": "15:28", "bemerkungen": "", "betriebsstelle": "XWS", "bremssystem": "", "fahrtrichtung": "11", "fzm_id": "380923", "gleis": "101", "gleis_ist": null, "gleis_plan": "101", "gleis_soll": "101", "ins_gegengleis": "0", "ist_durchfahrt": "0", "ist_kurzeinfahrt": "0", "mbr": "0", "triebfahrzeug": "646", "triebfahrzeug_ist": "646", "uebergang_nach_zug_id": "31050", "uebergang_von_zug_id": null, "verkehrstage": "Fr", "verkehrstage_bin": "0000100", "vmax": "120", "vmax_ist": "120", "wendezug": "0", "zug": "IRE 5029", "zug_id": "31046", "zuggattung": "IRE", "zuggattung_id": "15", "zugnummer": "5029" } ]

ECHO.
ECHO Send status message.
ECHO.

ECHO.
python.exe MQTTSender.py
ECHO.

PAUSE