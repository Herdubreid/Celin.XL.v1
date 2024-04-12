import { subjectStore } from './stores';

const loadSubjects = (filter: string) =>
  new Promise(async (resolve) => {
    if (filter?.trim().length > 0) {
      subjectStore.set(null);
      const unsubscribe = subjectStore.subscribe(value => {
        if (value) {
          resolve(value);
          unsubscribe();
        }
      });
      await global.blazorLib.invokeMethodAsync('SubjectLookupRequest', filter);
    } else {
      resolve([]);
    }
  });

export default loadSubjects;
