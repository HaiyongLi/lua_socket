function Add()    --求和
    a = SetDataByIdx(1);
	b = SetDataByIdx(2);
	b = a+b;
	GetData(b);
end

function Mutiply()   --求积
    a = SetDataByIdx(1);
	b = SetDataByIdx(2);
	b = a*b;
	GetData(b);
end

function Result()    --混合运算
    a = SetDataByIdx(1);
	b = SetDataByIdx(2);
	c = SetDataByIdx(3);
	b = a*(b+c);
	GetData(b);
end

function Average()   --求平均
    a = SetDataByIdx(1);
	b = SetDataByIdx(2);
	c = SetDataByIdx(3);
	d = SetDataByIdx(4);
	e = SetDataByIdx(5);
	b = (a+b+c+d+e)/5;
	GetData(b);
end

function mysum()   --求平均
	t = {1, 2, 3,4,5,6,7,8,9}
    for i in t do
		x = x+SetDataByIdx(i);
	end
	
	GetData(x);
end