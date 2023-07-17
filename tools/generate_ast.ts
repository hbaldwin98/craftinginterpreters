// get the args from the command line
const args = process.argv.slice(2);
let fs = require('fs');
if (args.length !== 1) {
    console.error('Usage: ts-node generate_ast.ts <output directory>');
}

type AstType = {
    name: string;
    fields: string[];
};

const outputDir = args[0];
defineAst(outputDir, 'Expression', [
    { name: 'Binary', fields: ['Expression left', 'Token op', 'Expression right'] },
    { name: 'Grouping', fields: ['Expression expr'] },
    { name: 'Literal', fields: ['object value'] },
    { name: 'Unary', fields: ['Token op', 'Expression right'] },
]);

defineAst(outputDir, 'Statement', [
    { name: 'StatementExpression', fields: ['Expression expr'] },
    { name: 'Print', fields: ['Expression expr'] },
]);

defineVisitorInterface(outputDir, 'Expression', [
    { name: 'Binary', fields: ['Expression left', 'Token op', 'Expression right'] },
    { name: 'Grouping', fields: ['Expression expr'] },
    { name: 'Literal', fields: ['object value'] },
    { name: 'Unary', fields: ['Token op', 'Expression right'] },
]);

defineVisitorInterface(outputDir, 'Statement', [
    { name: 'StatementExpression', fields: ['Expression expr'] },
    { name: 'Print', fields: ['Expression expr'] },
]);

function defineVisitorInterface(outputDir: string, baseName: string, types: AstType[]) {
    let output = `namespace cshlox;` + '\n\n';

    output += `public interface I${baseName}Visitor<T>` + '\n';
    output += '{' + '\n';

    types.forEach((type) => {
        output += '\tT Visit' + type.name + baseName + '(' + type.name + ' ' + baseName.toLowerCase() + ');' + '\n';
    });

    output += '}' + '\n';

    fs.writeFileSync(outputDir + `/I${baseName}Visitor.cs`, output);
}

function defineVisitor(baseName: string, name: string) {
    let output = `\tpublic override T Accept<T>(I${baseName}Visitor<T> visitor)\n\t{` + '\n';

    output += '\t\treturn visitor.Visit' + name + baseName + '(this);' + '\n';
    output += '\t}' + '\n';

    return output;
}

function defineAst(outputDir: string, baseName: string, types: AstType[], generateVisitor: boolean = true) {
    // make a new file for each type in c#
    // define our abstract base class
    let output = `namespace cshlox;` + '\n\n';
    output += `public abstract class ${baseName}\n{` + '\n';
    if (generateVisitor) {
        output += `\tpublic abstract T Accept<T>(I${baseName}Visitor<T> visitor);` + '\n';
    }
    output += '}' + '\n\n';

    fs.writeFileSync(outputDir + '/' + baseName + '.cs', output);

    types.forEach((type) => {
        let output = `namespace cshlox;` + '\n\n';

        output += `public class ${type.name} : ${baseName}\n{` + '\n';
        type.fields.forEach((field) => {
            let fields = field.split(' ');
            let fieldName = fields[1].charAt(0).toUpperCase() + fields[1].slice(1);
            output += `\tpublic ${fields[0]} ${fieldName} { get; }` + '\n';
        });

        output += '\n';
        output += `\tpublic ${type.name}(${type.fields.join(', ')})\n\t{` + '\n';
        type.fields.forEach((field) => {
            let fields = field.split(' ');
            let fieldName = fields[1].charAt(0).toUpperCase() + fields[1].slice(1);
            output += `\t\t${fieldName} = ${fields[1]};` + '\n';
        });

        output += '\t}' + '\n\n';
        if (generateVisitor) {
            output += defineVisitor(baseName, type.name);
        }
        output += '}';
        // create the file if it doesn't exist

        if (!fs.existsSync(outputDir)) {
            fs.mkdirSync(outputDir);
        }

        fs.writeFileSync(`${outputDir}/${type.name}.cs`, output);
    });
}
