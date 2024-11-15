﻿class Tools
{
    constructor(rawTools)
    {

    }
}

class Tiangan
{
    constructor(tools, index)
    {
        /**
         * @type {number}
         * @readonly
         */
        this.index = index;
    }
}

class Dizhi
{
    constructor(tools, index)
    {
        /**
         * @type {number}
         * @readonly
         */
        this.index = index;
    }
}

class NongliLunar
{
    constructor(tools, nongliLunar)
    {
        /**
         * @type {?Dizhi}
         * @readonly
         */
        this.nian = nongliLunar.nian === null ? null : new Dizhi(tools, nongliLunar.nian);
        /**
         * @type {?number}
         * @readonly
         */
        this.yue = nongliLunar.yue;
        /**
         * @type {?boolean}
         * @readonly
         */
        this.isRunyue = nongliLunar.isRunyue;
        /**
         * @type {?number}
         * @readonly
         */
        this.ri = nongliLunar.ri;
        /**
         * @type {?Dizhi}
         * @readonly
         */
        this.shi = nongliLunar.shi === null ? null : new Dizhi(tools, nongliLunar.shi);
    }
}

class NongliSolar
{
    constructor(tools, nongliSolar)
    {
        /**
         * @type {?Tiangan}
         * @readonly
        */
        this.niangan = nongliSolar.niangan === null ? null : new Tiangan(tools, nongliSolar.niangan);
        /**
         * @type {?Dizhi}
         * @readonly
        */
        this.nianzhi = nongliSolar.nianzhi === null ? null : new Dizhi(tools, nongliSolar.nianzhi);
        /**
         * @type {?Tiangan}
         * @readonly
        */
        this.yuegan = nongliSolar.yuegan === null ? null : new Tiangan(tools, nongliSolar.yuegan);
        /**
         * @type {?Dizhi}
         * @readonly
        */
        this.yuezhi = nongliSolar.yuezhi === null ? null : new Dizhi(tools, nongliSolar.yuezhi);
        /**
         * @type {?Tiangan}
         * @readonly
        */
        this.rigan = nongliSolar.rigan === null ? null : new Tiangan(tools, nongliSolar.rigan);
        /**
         * @type {?Dizhi}
         * @readonly
        */
        this.rizhi = nongliSolar.rizhi === null ? null : new Dizhi(tools, nongliSolar.rizhi);
        /**
         * @type {?Tiangan}
         * @readonly
        */
        this.shigan = nongliSolar.shigan === null ? null : new Tiangan(tools, nongliSolar.shigan);
        /**
         * @type {?Dizhi}
         * @readonly
        */
        this.shizhi = nongliSolar.shizhi === null ? null : new Dizhi(tools, nongliSolar.shizhi);
    }
}

class Outputs
{
    constructor()
    {
        /**
         * @type {?string}
         */
        this.error = "脚本实现不完善。请至少为 outputs.error 设置一个值。";
        /**
         * @type {string}
         */
        this.warning = "";
        /**
         * @type {number}
         */
        this.shanggua = 0;
        /**
         * @type {number}
         */
        this.xiagua = 0;
        /**
         * @type {number}
         */
        this.dongyao = 0;
    }
}

/**
 * @param {NongliLunar} nongliLunar
 * @param {NongliSolar} nongliSolar
 * @param {?Date} gregorianCalendar
 * @param {string} shangguaInput
 * @param {string} xiaguaInput
 * @param {string} dongyaoInput
 * @param {Outputs} outputs
 * @param {Tools} tools
 */
function THE_FUNCTION_YOU_ARE_GOING_TO_IMPLEMENT(
    nongliLunar, nongliSolar, gregorianCalendar,
    shangguaInput, xiaguaInput, dongyaoInput,
    outputs, tools)
{
    // 事实上它是通过 eval 执行的，而不是调用函数。
    // 其运行结果通过 outputs 的属性来向外传递，而不使用 return （也不会取 eval 的返回值）。
    // 但有时候用 return 进行跳转很方便，可以增加一个内部方法来解决。

    // 下面是一个示例（也是默认的脚本）：
    function calculate()
    {
        let 年 = nongliLunar.nian.index;
        let 月 = nongliLunar.yue;
        let 日 = nongliLunar.ri;
        let 时 = nongliLunar.shi.index;
    
        try
        {
            outputs.shanggua = eval(shangguaInput);
        }
        catch (ex)
        {
            outputs.error = `上卦计算失败。请检查输入的时间是否完整，以及上卦表达式是否正确。详细信息：${ex}`
            return;
        }
        let 上 = outputs.shanggua;
        
        try
        {
            outputs.shanggua = eval(shangguaInput);
        }
        catch (ex)
        {
            outputs.error = `下卦计算失败。请检查输入的时间是否完整，以及下卦表达式是否正确。详细信息：${ex}`
            return;
        }
        let 下 = eval(xiaguaInput)
    
        try
        {
            outputs.dongyao = eval(dongyaoInput);
        }
        catch (ex)
        {
            outputs.error = `动爻计算失败。请检查输入的时间是否完整，以及动爻表达式是否正确。详细信息：${ex}`
            return;
        }
    
        if (!Number.isInteger(outputs.shanggua) ||
            !Number.isInteger(outputs.xiagua) ||
            !Number.isInteger(outputs.dongyao))
        {
            outputs.error = `计算得到的卦数并非整数。其中上卦为 ${outputs.shanggua} ，下卦为 ${outputs.xiagua} ，动爻为 ${outputs.dongyao} 。`;
            return;
        }
        
        outputs.error = null;
    }
    calculate()
}

export function calculate(rawInputs, rawTools)
{
    const tools = new Tools(rawTools);
    const nongliLunar = new NongliLunar(tools, rawInputs.nongliLunar);
    const nongliSolar = new NongliSolar(tools, rawInputs.nongliSolar);
    const gregorianCalendar = rawInputs.gregorianCalendar;
    const shangguaInput = rawInputs.shanggua;
    const xiaguaInput = rawInputs.xiagua;
    const dongyaoInput = rawInputs.dongyao;
    const outputs = new Outputs();

    try
    {
        eval(rawInputs.script);
    }
    catch (ex)
    {
        return {
            error: `在执行脚本时发生了异常。请注意，正确的脚本本身不应该抛出错误，而是通过 outputs.error 传递消息，因此这可能意味着脚本本身并不完善。详细信息：${ex}`,
            warning: "",
            shanggua: 0,
            xiagua: 0,
            dongyao: 0
        }
    }

    if (outputs.error === null)
    {
        if (Number.isInteger(outputs.shanggua) &&
            Number.isInteger(outputs.xiagua) &&
            Number.isInteger(outputs.dongyao))
        {
            return {
                error: null,
                warning: outputs.warning.toString(),
                shanggua: outputs.shanggua,
                xiagua: outputs.xiagua,
                dongyao: outputs.dongyao
            }
        }
        else
        {
            return {
                error: `脚本输出的卦数并非整数。请注意，正确的脚本本身就应该对此进行检查，因此这可能意味着脚本本身并不完善。详细信息：得到的上卦为 ${outputs.shanggua} ，下卦为 ${outputs.xiagua} ，动爻为 ${outputs.dongyao} 。`,
                warning: "",
                shanggua: 0,
                xiagua: 0,
                dongyao: 0
            }
        }
    }
    return {
        error: outputs.error.toString(),
        warning: "",
        shanggua: 0,
        xiagua: 0,
        dongyao: 0
    }
}