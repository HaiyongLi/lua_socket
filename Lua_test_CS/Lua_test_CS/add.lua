function Add()    --���
    a = SetDataByIdx(1);
	b = SetDataByIdx(2);
	b = a+b;
	GetData(b);
end

function Mutiply()   --���
    a = SetDataByIdx(1);
	b = SetDataByIdx(2);
	b = a*b;
	GetData(b);
end

function Result()    --�������
    a = SetDataByIdx(1);
	b = SetDataByIdx(2);
	c = SetDataByIdx(3);
	b = a*(b+c);
	GetData(b);
end

function Average()   --��ƽ��
    a = SetDataByIdx(1);
	b = SetDataByIdx(2);
	c = SetDataByIdx(3);
	d = SetDataByIdx(4);
	e = SetDataByIdx(5);
	b = (a+b+c+d+e)/5;
	GetData(b);
end

function mysum()   --��ƽ��
	t = {1, 2, 3,4,5,6,7,8,9}
    for i in t do
		x = x+SetDataByIdx(i);
	end
	
	GetData(x);
end