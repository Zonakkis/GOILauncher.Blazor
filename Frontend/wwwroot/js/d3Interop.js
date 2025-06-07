let playerColorScale;

window.updateChart = function (elementId, data, date) {
    // 确保 D3 已加载后再初始化颜色比例尺
    if (!playerColorScale) {
        playerColorScale = d3.scaleOrdinal(d3.schemeCategory10);
    }
    
    console.log("data:", data);
    console.log("date:", date);

    const container = document.getElementById(elementId);
    const width = container.clientWidth;
    const height = container.clientHeight;

    const svg = d3.select("#" + elementId).select("svg");
    const isFirstRender = svg.empty();

    // 如果是第一次渲染，创建 SVG
    if (isFirstRender) {
        d3.select("#" + elementId)
            .append("svg")
            .attr("viewBox", [0, 0, width, height])
            .attr("preserveAspectRatio", "xMinYMin meet")   // ← 新增：左上对齐，不留白
            // 如果想强制拉伸，可换成 "none"
            .style("width", "100%")
            .style("height", "100%");
    }

    const currentSvg = d3.select("#" + elementId).select("svg");

    const marginLeft = 20;  // 从20增加到40，给排名数字留空间
    const marginRight = 140; // 大幅增加右边距，给时间显示留空间

    const x = d3.scaleLinear()
        .domain([0, d3.max(data, d => d.value)])
        .range([0, width - marginLeft - marginRight]); // 减去左右边距

    const y = d3.scaleBand()
        .domain(data.map(d => d.name))
        .range([0, height])
        .padding(0.1);

    const yAxis = d3.axisLeft(y)
        .tickFormat((d, i) => i + 1)
        .tickSize(0);

    // 把所有出现过的玩家名加入到比例尺 domain，去重后赋值
    playerColorScale.domain([
        ...new Set([
            ...playerColorScale.domain(),
            ...data.map(d => d.name)
        ])
    ]);

    // 后续统一使用 playerColorScale
    const colorScale = playerColorScale;

    // 绘制条形 - 使用数据绑定和过渡动画
    // 1. 只选带 .bar 的 rect
    const bars = currentSvg.selectAll('rect.bar')
        .data(data, d => d.name);

    bars.join(
        // enter
        enter => enter.append('rect')
            .attr('class', 'bar')
            .attr('x', marginLeft)
            .attr('y', height)            // 从底部开始
            .attr('width', d => x(d.value))
            .attr('height', y.bandwidth())
            .style('fill', d => colorScale(d.name))
            .call(enter => enter.transition()
                .duration(800)
                .attr('width', d => x(d.value))
                .attr('y', d => y(d.name))
            ),

        // update
        update => update.call(update => update.transition()
            .duration(800)
            .attr('width', d => x(d.value))
            .attr('y', d => y(d.name))
            .attr('height', y.bandwidth())
            .style('fill', d => colorScale(d.name))
        ),

        // exit
        exit => exit.call(exit => exit
            .transition()
            .duration(800)
            .attr('y', height)           // 飞到底部
            .style('opacity', 0)         // 再渐隐
            .remove()
        )
    );

    // 绘制左轴 - 固定排名位置
    let yAxisGroup = currentSvg.select(".y-axis");
    if (yAxisGroup.empty()) {
        yAxisGroup = currentSvg.append("g").attr("class", "y-axis");
    }

    yAxisGroup
        .attr("transform", `translate(${marginLeft},0)`) // y轴位置会更靠左
        .call(yAxis); // 移除transition，让排名立即更新到正确位置

    yAxisGroup.selectAll(".tick text")
        .style("font-size", "18px");

    // 添加日期显示
    if (date) {
        let dateLabel = currentSvg.select(".date-label");
        if (dateLabel.empty()) {
            dateLabel = currentSvg.append("text")
                .attr("class", "date-label");
        }

        dateLabel
            .attr("x", width - marginRight + 10) // 调整到右边距范围内
            .attr("y", 25) // 距离顶部25px
            .attr("text-anchor", "start") // 改为左对齐
            .attr("fill", "black")
            .attr("font-size", "16px")
            .attr("font-weight", "bold")
            .text(formatDate(date));
    }

    // 在绘制名字之前，先绘制头像
    const iconSize = 24;      // 头像尺寸
    const iconPadding = 4;    // 头像与文字之间的间距

    const avatars = currentSvg.selectAll('image.avatar')
        .data(data, d => d.name);

    avatars.join(
        enter => enter.append('image')
            .attr('class', 'avatar')
            .attr('x', marginLeft + 4)
            .attr('y', height)                                // 初始从底部开始
            .attr('width', iconSize)
            .attr('height', iconSize)
            .attr('opacity', 0)                               // 初始透明
            .attr('href', d => d.avatarUrl)
            .call(ent => ent.transition()
                .duration(800)
                .attr('y', d => y(d.name) + (y.bandwidth() - iconSize) / 2)  // 飞到条形中央
                .attr('opacity', 1)                                         // 渐显
            ),

        update => update.call(u => u.transition()
            .duration(800)
            .attr('x', marginLeft + 4)
            .attr('y', d => y(d.name) + (y.bandwidth() - iconSize) / 2)
            .attr('href', d => d.avatarUrl)
        ),

        exit => exit.call(e => e.transition()
            .duration(800)
            .attr('y', height)
            .attr('opacity', 0)
            .remove()
        )
    );

    // 条形内部标签
    const barLabels = currentSvg.selectAll("text.bar-label")
        .data(data, d => d.name);

    // 把条形内部文字往右偏移，给头像留空间
    barLabels.enter()
        .append("text")
        .attr("class", "bar-label")
        .attr("x", marginLeft + iconSize + iconPadding + 4)
        .attr("y", height)
        .attr("dy", "0.35em")
        .attr("text-anchor", "start")
        .attr("fill", "white")               // ← 固定白色
        .attr("font-size", "16px")
        .attr("opacity", 0)
        .text(d => d.name)
        .transition()
        .duration(800)
        .attr("y", d => y(d.name) + y.bandwidth() / 2)
        .attr("opacity", 1);

    barLabels.transition()
        .duration(800)
        .attr("x", marginLeft + iconSize + iconPadding + 4)
        .attr("y", d => y(d.name) + y.bandwidth() / 2)
        .attr("fill", "white")               // ← 同步更新填充色
        .text(d => d.name);

    barLabels.exit()
        .transition()
        .duration(800)
        .attr("y", height)
        .attr("opacity", 0)
        .remove();

    // 条形内部右侧平台标签
    const barPlatformLabels = currentSvg.selectAll("text.bar-platform")
        .data(data, d => d.name);

    barPlatformLabels.enter()
        .append("text")
        .attr("class", "bar-platform")
        .attr("x", d => marginLeft + x(d.value) - 8) // 在条形右端内侧
        .attr("y", height)
        .attr("dy", "0.35em")
        .attr("text-anchor", "end") // 右对齐
        .attr("fill", "white")
        .attr("font-size", "14px")
        .attr("opacity", 0)
        .text(d => d.platform || "")
        .transition()
        .duration(800)
        .attr("y", d => y(d.name) + y.bandwidth() / 2)
        .attr("opacity", 1);

    barPlatformLabels.transition()
        .duration(800)
        .attr("x", d => marginLeft + x(d.value) - 8)
        .attr("y", d => y(d.name) + y.bandwidth() / 2)
        .text(d => d.platform || "");

    barPlatformLabels.exit()
        .transition()
        .duration(800)
        .attr("y", height)
        .attr("opacity", 0)
        .remove();


    // 条形右侧数值
    const barValues = currentSvg.selectAll("text.bar-value")
        .data(data, d => d.name);

    barValues.enter()
        .append("text")
        .attr("class", "bar-value")
        .attr("x", d => marginLeft + x(d.value) + 8)
        .attr("y", height)
        .attr("dy", "0.35em")
        .attr("text-anchor", "start")
        .attr("fill", "black")               // ← 也固定白色或根据背景选黑色
        .attr("font-size", "16px")
        .attr("opacity", 0)
        .text(d => formatTime(d.value))
        .transition()
        .duration(800)
        .attr("y", d => y(d.name) + y.bandwidth() / 2)
        .attr("opacity", 1);

    barValues.transition()
        .duration(800)
        .attr("x", d => marginLeft + x(d.value) + 8)
        .attr("y", d => y(d.name) + y.bandwidth() / 2)
        .attr("fill", "black")               // ← 同步更新填充色
        .text(d => formatTime(d.value));

    barValues.exit()
        .transition()
        .duration(800)
        .attr("y", height) // 退出时飞到底部
        .attr("opacity", 0)
        .remove();

    // 添加日期显示
    if (date) {
        let dateLabel = currentSvg.select(".date-label");
        if (dateLabel.empty()) {
            dateLabel = currentSvg.append("text")
                .attr("class", "date-label");
        }

        dateLabel
            .attr("x", width - marginRight + 10) // 调整到右边距范围内
            .attr("y", 25) // 距离顶部25px
            .attr("text-anchor", "start") // 改为左对齐
            .attr("fill", "black")
            .attr("font-size", "16px")
            .attr("font-weight", "bold")
            .text(formatDate(date));
    }
}

function formatTime(time) {
    const minutes = Math.floor(time / 60);
    const seconds = Math.floor(time % 60);                       // 保证秒数为整数
    const milliseconds = Math.floor((time - minutes * 60 - seconds) * 1000);
    return minutes > 0
        ? `${minutes}分${seconds.toString().padStart(2, '0')}.${milliseconds.toString().padStart(3, '0')}秒` :
        `${seconds}.${milliseconds.toString().padStart(3, '0')}秒`;
}

// 添加日期格式化函数
function formatDate(date) {
    if (typeof date === 'string') {
        date = new Date(date);
    }

    const year = date.getFullYear();
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const day = date.getDate().toString().padStart(2, '0');

    return `${year}年${month}月${day}日`;
}