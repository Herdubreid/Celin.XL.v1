const cql = [
    "f0005 all(sy=41 rt=L)",
    "f4108 (mcu,litm,lotn,mmej) all(lotn=00193326130010125692)",
    "F4108 [group(lotn) count(lotn)] having[count(lotn>1)]",
    "f9860 (obnm,md) all(obnm^F41)",
    "f4211 (mcu,doco,lnid,shpn,drqj,nxtr,litm,lotn,uorg,uom,sqor,uom2,pqor,uom1)\nall(nxtr=540)",
    "f4211 [group(uom,uom2,uom1,lotn) count(lnid)] all(nxtr=540)",
    "v41021e (mcu,litm,locn,lotn,lots,pqoh,sqoh,uom1,uom2,dual) all(f4101.litm=CWLPK0002 f41021.mcu=MHPE)",
    "V5541280 (doco,lnid,litm,f4211.mcu,f4211.locn,f4211.lotn,nxtr,pqoh,soqs,uom,sqor,uom2)\nall(f4211.doco=101633 f4211.nxtr bw 540,560)",
    "f41021 [group(mcu,locn) count(itm)] all(pqoh>0 lotn!)",
    "f4801 (doco,mcu,litm,srst) all(srst bw 45,70 litm=CDSLS0004)",
    "f4102 (mcu,itm,litm) all(litm=CDSLS0004)",
    "f4111 (dct,litm,mcu,locn,lotn,dgl,trqt,trum,sqor,uom2,user,pid,crdj,tday) all(dct=IT lotn=00293326130021815107)",
    "f4111 [max(trqt)] all(dct=IT user^T)",
    "f4111 all(dct=IT trqt=240000)",
    "f3111 (doco,cmcu,cpil,uorg,trqt) all(doco=3003488)"
];

const cqlLookup = (text, textareaEl) => {
    const toMatch = (text
        .replaceAll("\n", " ")
        .match(/(?:.*;)*(.*)$/) ?? ["", ""])[1].trimLeft().toUpperCase();
    if (toMatch.length > 0 && text.length === textareaEl?.selectionEnd) {
        const m = cql
            .filter(c => c.toUpperCase().startsWith(toMatch))
            .map(c => c.slice(toMatch.length));
        return m;
    }

    return [];
}

export default cqlLookup;

