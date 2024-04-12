export const selectedCode = (code, start, end) => {
  if (!code || code.length === 0) return { before: "", snippet: "" };

  const left = code.slice(0, start);
  const right = code.slice(start);
  const ndx = left.lastIndexOf("\n\n");
  let from = start === end ? ndx === -1 ? 0 : ndx + 2 : start;
  const to = start === end ? right.search(/^\n/m) : end - start;

  const snippet = (left.slice(from) + right.slice(0, to)).trimEnd();
  const before = left.slice(0, from).replace(/./g, " ");

  return {
    before,
    snippet,
  };
};
