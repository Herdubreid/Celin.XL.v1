import { subjectDemoStore } from "./stores";

const loadSubjectDemo = (subject) =>
    new Promise(resolve => {
        // subjectDemoStore.set(null);
        const unsubscribe = subjectDemoStore.subscribe(value => {
            if (value) {
                resolve(value);
                unsubscribe();
            }
        });
        global.blazorLib.invokeMethodAsync('SubjectDemoLookupRequest', subject);
    });

export default loadSubjectDemo;