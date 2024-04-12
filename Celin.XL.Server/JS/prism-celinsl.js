export const celinsl = {
    string: /"[^"]*"/,
    comment: /\/\*[^*]*\*\//,
    bold: /#\w+|@\w+/,
    variable: /\$(?:row|col\[\d+\]|form|title|datacol|gridcol\[\d+\]|records)/,
    keyword: /\b(?:do|set|select|qbe|open|action|output|each|close|demo|dump|data|grid|insert|update|continue|breakOnError|radio)\b/,
    punctuation: /[{}[\];(),\.]/,
    number: /\b\d+\b/,
};
